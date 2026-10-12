using Backlot.Core.Services;
using Backlot.Services.Filesystem.LocalDiskStorage;
using Backlot.Testing.Core;
using NSubstitute;
using NUnit.Framework;

namespace Backlot.Testing.Defaults.Instructing.MustachExpressionEngine;

public class Execute
{
    private Backlot.Defaults.Instructing.MustachExpressionEngine _expressionEngine;
    [SetUp]
    public void Setup()
    {
        _expressionEngine = new Backlot.Defaults.Instructing.MustachExpressionEngine();
    }
    
    [Test]
    public void Execute_WithTypedActor_CalculatedValue()
    {
        // Arrange
        string template = "{{Number1}}_{{Number2}}_{{Operation}}_{{Uid}}";
        
        var formula = new Formula()
        {
            Number1 = 7,
            Number2 = 9,
            Operation = "sum",
            Uid = "023d02600ded"
        };
        
        // Act
        var value = _expressionEngine.Execute(template, formula);
        
        // Assert
        Assert.That(value, Is.EqualTo("7_9_sum_023d02600ded"));
    }
    
}