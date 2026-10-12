using Backlot.Core.Abstraction.Actors;
using Backlot.Core.Services;
using Backlot.Services.Filesystem.LocalDiskStorage;
using Backlot.Testing.Shared.TestModels;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Backlot.Testing.Core.Abstraction.Actors.Acting;

public class Presents
{
    [SetUp]
    public void Setup()
    {
        Initialize.Setup((builder) => new AliasDirector(new LocalDiskStorage(), Substitute.For<IConfigurationManager>(), builder));
    }
    
    [Test]
    public void Presents_WithActorNotHavingAUidAndARoleAliasReferringToAnExpressionEngine_UidIsCalculatedBasedOnExpressionEngine()
    {
        // Arrange
        var actor = JObject.FromObject(new
        {
            Number1 = 7,
            Number2 = 9,
            Operation = "sum",
            Name = "sumexample",
            FormulaId = "12",
        });

        // Act
        var role = actor.Presents<IFormula>();
        
        // Assert
        Assert.That(role.Uid, Is.EqualTo("12_sumexample!"));
    }
}