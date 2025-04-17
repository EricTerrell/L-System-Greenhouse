using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using L_System_Greenhouse.TurtleGraphics;
using L_System_Greenhouse.Utils;
using L_System_Greenhouse.ViewModels;
using TurtleGraphicsState = L_System_Greenhouse.ViewModels.TurtleGraphicsState;
using log4net;

namespace L_System_Greenhouse;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

    private Bitmap? _bitmap;

    public Bitmap? Bitmap
    {
        get => _bitmap;
        set
        {
            _bitmap = value;
            OnPropertyChanged();
        }
    }
    
    private HelpWindow? _helpWindow;

    private IStorageFile _file;

    private CancellationTokenSource? _cancellationTokenSource;

    private ViewModels.LSystem _uiLSystem;
    
    public ViewModels.LSystem UilSystem
    {
        get => _uiLSystem;

        set
        {
            _uiLSystem = value;
            OnPropertyChanged();
        }
    }

    private bool _isDrawing;
    
    public bool IsDrawing 
    { 
        get => _isDrawing;
        private set
        {
            _isDrawing = value;
            
            CanDraw = !_isDrawing && AxiomTB.Text.Trim().Length > 0;
            
            OnPropertyChanged();
        } 
    }

    private bool _canDraw;
    
    public bool CanDraw 
    { 
        get => _canDraw;
        private set
        {
            _canDraw = value;
            OnPropertyChanged();
        } 
    }

    public TurtleGraphicsState TurtleGraphicsStateUI { get; set; } = new();
    
    public new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public MainWindow()
    {
        InitializeComponent();
        
        new L_System_Greenhouse.TurtleGraphics.TurtleGraphicsState().ToUI(TurtleGraphicsStateUI);
        
        DataContext = this;

        _uiLSystem = new ViewModels.LSystem
        {
            Axiom       = "F",
            Iterations  = 4,
            Productions = [new Production { Letter = "F", ReplacementLetters = "F[+FF][-FF]F[-F][+F]F" }]
        };

        // Need to ensure that UI updates properly.
        UilSystem = _uiLSystem;
        
        // Simulate a Draw button click on the UI thread.
        Dispatcher.UIThread.Post(() => Draw_OnClick(null, null));
    }

    private void AddProduction_OnClick(object? sender, RoutedEventArgs e)
    {
        UilSystem.Productions.Add(new Production());
    }

    private void DeleteProduction_OnClick(object? sender, RoutedEventArgs e)
    {
        UilSystem.Productions.Remove(ProductionsGrid.SelectedItem as Production);
    }

    private static FilePickerFileType AllFilesFileFilter { get; } = new("All Files")
    {
        Patterns = ["*.*"]
    };

    private static FilePickerFileType LSGHFileFilter { get; } = new($"{StringLiterals.AppName} Files")
    {
        Patterns = [StringLiterals.WildcardedFileType]
    };

    private static FilePickerFileType BitmapFileFilter { get; } = new("L-System Bitmap Files")
    {
        Patterns = ["*.png"]
    };

    private static FilePickerFileType LSGHAll { get; } = new($"{StringLiterals.AbbreviatedAppName} File")
    {
        Patterns = [ StringLiterals.WildcardedFileType ],
        AppleUniformTypeIdentifiers = [ "public.image" ],
        MimeTypes = ["application/octet-stream"]
    };

    private async void Draw_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            CanResize = false;
            
            _cancellationTokenSource?.Dispose();
            
            _cancellationTokenSource = new CancellationTokenSource();

            Status.Content = "Processing...";

            IsDrawing = true;

            var lSystem = LSystem.LSystem.FromUI(UilSystem);

            var turtleGraphicsState =
                L_System_Greenhouse.TurtleGraphics.TurtleGraphicsState.FromUI(TurtleGraphicsStateUI);

            var pens = new PenPool((int)LineWidthUD.Value);
            
            var cancellationToken = _cancellationTokenSource.Token;

            var computeStartTime = DateTime.Now;

            var lineCount = 0;

            Bitmap?.Dispose();
            Bitmap ??= null;
            
            var bitmap = await Task.Run(() =>
                {
                    try
                    {
                        var progress = new Progress<string>(progressMessage =>
                        {
                            Dispatcher.UIThread.Post(() => { Status.Content = progressMessage; });
                        });

                        var lSystemOutput = lSystem.Rewrite(UilSystem.Iterations, cancellationToken, progress);

                        cancellationToken.ThrowIfCancellationRequested();

                        var turtleCommands = ConvertToTurtleGraphics
                            .Convert(lSystemOutput.ToList(), turtleGraphicsState, cancellationToken, progress);

                        cancellationToken.ThrowIfCancellationRequested();

                        var lineData = new ConvertToLineData()
                            .Convert(turtleCommands, GraphicsSurface.Bounds, cancellationToken, progress);

                        cancellationToken.ThrowIfCancellationRequested();

                        var bitmap = new RenderTargetBitmap(
                            new PixelSize((int)GraphicsSurface.Bounds.Width, (int)GraphicsSurface.Bounds.Height));
                        using var ctx = bitmap.CreateDrawingContext(true);

                        ctx.FillRectangle(Brushes.Black, new Rect(0, 0, Bounds.Width, Bounds.Height));

                        var linesProcessed = 0;
                        
                        lineData.ForEach(line =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            linesProcessed++;

                            if (linesProcessed == 1 || linesProcessed % 1_000_000 == 0)
                            {
                                var message = $"Drawing line {linesProcessed:N0} of {lineData.Count:N0}";
                                
                                Dispatcher.UIThread.Post(() => { Status.Content = message; });
                            }
                            
                            ctx.DrawLine(pens.GetPen(line.PenThickness), line.Points[0], line.Points[1]);
                        });

                        lineCount = lineData.Count;
                        
                        return bitmap;
                    }
                    catch (OperationCanceledException ex)
                    {
                        Log.Info("User Cancelled", ex);    

                        return null;
                    }
                });
            
            var computeTime = DateTime.Now - computeStartTime;

            Bitmap = bitmap;
            
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                // Draw
                GraphicsSurface.Draw(bitmap);
                
                Status.Content = $"Lines: {lineCount:N0}\tTime: {FormatUtils.FormatTimeSpan(computeTime)}";
            }
        }
        finally
        {
            CanResize = true;
            IsDrawing = false;
            
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                Status.Content = "Cancelled";
            }
        }
    }

    private void Cancel_OnClick(object? sender, RoutedEventArgs e)
    {
        Status.Content = "Cancelling...";
        
        // Cancel the cancellation token
        _cancellationTokenSource.Cancel();
    }

    private void HandleHelpClosed(object? sender, EventArgs e)
    {
        HelpHelp.IsEnabled = true;
    }
    
    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
            _helpWindow?.Close();
    }

    private void AxiomTB_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        CanDraw = AxiomTB.Text.Trim().Length > 0 && !_isDrawing;
    }

    private void HelpHelp_OnClick(object? sender, RoutedEventArgs e)
    {
        _helpWindow = new HelpWindow();

        _helpWindow.Closed += HandleHelpClosed;

        HelpHelp.IsEnabled = false;
        
        _helpWindow.Show();
    }

    private void FileExit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void FileOpen_OnClick(object? sender, RoutedEventArgs e)
    {
        // Start async operation to open the dialog.
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = $"Open {StringLiterals.AppName} File",
            AllowMultiple = false,
            FileTypeFilter = [ LSGHAll, FilePickerFileTypes.All ]
        });

        if (files.Count >= 1)
        {
            _file = files[0];
            
            // Open reading stream from the first file.
            await using var readFileStream = await _file.OpenReadAsync();

            using (BinaryReader binaryReader = new(readFileStream))
            {
                var document =
                    System.Text.Json.JsonSerializer
                        .Deserialize<Document>(binaryReader.ReadString());
                
                document.LSystem.ToUI(UilSystem);
                document.TurtleGraphicsState.ToUI(TurtleGraphicsStateUI);

                Title = $"{StringLiterals.AppName} - {_file.Name}";
                
                Draw_OnClick(null, new RoutedEventArgs());
            }
        }
    }

    private async void WriteFile()
    {
        await using var writeFileStream = await _file.OpenWriteAsync();
        await using BinaryWriter binaryWriter = new(writeFileStream);
        
        var document = new Document(
            LSystem.LSystem.FromUI(UilSystem), 
            L_System_Greenhouse.TurtleGraphics.TurtleGraphicsState.FromUI(TurtleGraphicsStateUI));
            
        var serializedData = System.Text.Json.JsonSerializer.Serialize(document);

        binaryWriter.Write(serializedData);
        
        Title = $"{StringLiterals.AppName} - {_file.Name}";
    }
    
    private void FileSave_OnClick(object? sender, RoutedEventArgs e)
    {
        var fileExists = false;

        if (_file != null)
        {
            var path = _file.TryGetLocalPath();
            fileExists = path != null && File.Exists(path);
        }

        // Write the file if it actually exists. Otherwise, do a "Save As".
        if (fileExists)
        {
            WriteFile();
        }
        else
        {
            FileSaveAs_OnClick(null, null);
        }
    }

    private async void FileSaveAs_OnClick(object? sender, RoutedEventArgs e)
    {
        // Start async operation to open the dialog.
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = $"Save {StringLiterals.AppName} File",
            FileTypeChoices = [ LSGHFileFilter, AllFilesFileFilter ]
        });

        if (file != null)
        {
            _file = file;

            WriteFile();
        }
    }

    private void HelpAbout_OnClick(object? sender, RoutedEventArgs e)
    {
        new AboutDialog().ShowDialog(this);
    }

    private async void SaveBitmap_OnClick(object? sender, RoutedEventArgs e)
    {
        // Start async operation to open the dialog.
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save L-System Bitmap File",
            FileTypeChoices = [ BitmapFileFilter, AllFilesFileFilter ]
        });

        if (file != null)
        {
            Bitmap?.Save(file.TryGetLocalPath());
        }
    }
}