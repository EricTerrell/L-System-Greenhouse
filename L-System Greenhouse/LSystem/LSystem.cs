﻿using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using log4net;

namespace L_System_Greenhouse.LSystem;

public class LSystem
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

    public LSystem(List<Production> productions, string axiom, int iterations)
    {
        Productions = productions;
        Axiom       = axiom;
        Iterations  = iterations;
    }
    
    public static LSystem FromUI(ViewModels.LSystem uiLSystem)
    {
        var productions = new List<Production>();
        
        var lettersAdded = string.Empty;
        
        uiLSystem.Productions.ToList().ForEach(uiProduction =>
        {
            if (!string.IsNullOrEmpty(uiProduction.Letter) && !lettersAdded.Contains(uiProduction.Letter))
            {
                productions.Add(new Production(uiProduction.Letter[0], uiProduction.ReplacementLetters));
                lettersAdded += uiProduction.Letter[0];
            }
        });
        
        return new LSystem(productions, uiLSystem.Axiom, uiLSystem.Iterations);
    }

    public void ToUI(ViewModels.LSystem uiLSystem)
    {
        uiLSystem.Axiom      = Axiom;
        uiLSystem.Iterations = Iterations;

        uiLSystem.Productions = [];
        
        Productions.ForEach(production =>
        {
            var uiProduction = new ViewModels.Production();
            
            uiProduction.Letter = production.Letter.ToString();
            uiProduction.ReplacementLetters = production.ReplacementLetters;
            
            uiLSystem.Productions.Add(uiProduction);
        });
    }

    public string Axiom { get; }

    public List<Production> Productions { get; }

    public int Iterations { get; }

    public string Rewrite(CancellationToken cancellationToken)
    {
        return Rewrite(Iterations, cancellationToken);
    }

    public string Rewrite(int iterations, CancellationToken cancellationToken, IProgress<string>? progress = null)
    {
        var startTime = DateTime.Now;
        
        var currentAxiom = Axiom;

        try
        {
            // If multiple productions start with the same letter, just use one of those productions.
            var productionDictionary = 
                Productions
                    .ToDictionary(p => p.Letter, p => p.ReplacementLetters);

            for (var i = 0; i < iterations; i++)
            {
                progress?.Report($"Rewriting: Iteration {i} of {iterations}");
                
                cancellationToken.ThrowIfCancellationRequested();

                currentAxiom = string.Join(
                    string.Empty,
                    from letter in currentAxiom
                    select
                        productionDictionary.GetValueOrDefault(letter, letter.ToString()));
            }
        }
        catch (OperationCanceledException)
        {
            currentAxiom = string.Empty;
        }

        Log.Info($"LSystem.Rewrite: elapsed time: {DateTime.Now - startTime}");

        return currentAxiom;
    }
}