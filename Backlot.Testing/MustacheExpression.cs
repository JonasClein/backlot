using Backlot.Core.Abstraction.Actors;
using Backlot.Core.Services;
using Backlot.Services.Filesystem.LocalDiskStorage;
using Backlot.Testing.Core;
using Backlot.Testing.Shared.TestModels;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Backlot.Testing;

/// <summary>
/// Test to check if intercepting using the backlot interceptors works correctly.
/// </summary>
public class MustachExpression
{
    [SetUp]
    public void Setup()
    {
        Initialize.Setup((builder) => new AliasDirector(new LocalDiskStorage(), Substitute.For<IConfigurationManager>(), builder));
    }
    
    [Test]
    public void Presents_WithActorNotHavingAUidAndARoleAliasReferingToAExpressionEngine_UidIsCalculatedBasedOnExpressionEngine()
    {
        // arrange:
        var actor = JObject.FromObject(new
        {
            Number1 = 7,
            Number2 = 9,
            Operation = "sum",
            Name = "sumexample",
            FormulaId = "12",
        });

        // act
        var role = actor.Presents<IFormula>();
        
        // assert:
        Assert.That(role.Uid == "12_sumexample!");
    }
}