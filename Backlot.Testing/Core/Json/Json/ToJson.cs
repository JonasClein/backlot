using Backlot.Core.Abstraction.Actors;
using Backlot.Core.Json;
using Backlot.Core.Json.Serialization.Newtonsoft;
using Backlot.Testing.Shared.TestModels;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Backlot.Testing.Core.Json.Json;

/// <summary>
/// Tests JSON serialization of roles using different
/// serialization strategies and property values.
/// </summary>
public class ToJson
{
    [SetUp]
    public void Setup()
    {
        Initialize.Setup();
    }
    
    [Test]
    public void ToJson_WithJSONActorAndSerializedForPersistence_CalculatedFieldNotSerialized()
    {
        // Arrange
        var origin = JObject.FromObject(new
        {
            Number1 = 7,
            Number2 = 9,
            Number3 = 4,
            Operation = "sum",
        });
        
        var role = origin.Presents<IFormula>();
        role.Number3 = 4;

        // Act
        var jsn = Backlot.Core.Json.Json.ToJson(role, Strategy.SerializeForPersistance);
        
        // Assert
        Assert.That(jsn, Does.Not.Contain("Number3"));
    }
    
    [Test]
    public void ToJson_WithJSONActorAndSerializedForInteraction_CalculatedFieldSerialized()
    {
        // Arrange
        var origin = JObject.FromObject(new
        {
            Number1 = 7,
            Number2 = 9,
            Operation = "sum",
        });
        var role = origin.Presents<IFormula>();
        role.Number3 = 4;

        // Act
        var jsn = Backlot.Core.Json.Json.ToJson(role, Strategy.SerializeForInteraction);
        
        // Assert
        Assert.That(jsn,  Does.Contain("Number3"));
    }
    
    [Test]
    public void ToJson_WithTypedActorWithUpdatedCalculatedFieldAndSerializeForInteraction_CalculatedFieldSerialized()
    {
        // Arrange
        var origin = new FormulaCalculatedField()
        {

            Number1 = 7,
            Number2 = 9,
            Number3 = 4,
            Operation = "sum",
        };
        
        var role = origin.Presents<IFormula>();
        // update calculated field
        role.Number3 = 7;

        // Act
        var jsn = Backlot.Core.Json.Json.ToJson(role, Strategy.SerializeForInteraction);
        
        // Assert
        Assert.That(jsn, Does.Contain("Number3"));
    }
    
    [Test]
    public void ToJson_WithTypedActorWithCalculatedFieldOriginallyGivenAndSerializeForInteraction_CalculatedFieldIsNotSerialized()
    {
        // Arrange
        var origin = new FormulaCalculatedField()
        {

            Number1 = 7,
            Number2 = 9,
            Number3 = 4,
            Operation = "sum",
        };
        
        var role = origin.Presents<IFormula>();

        // Act
        var jsn = Backlot.Core.Json.Json.ToJson(role, Strategy.SerializeForInteraction);
        
        // Assert
        Assert.That(jsn, Does.Not.Contain("Number3"));
    }
    
    [Test]
    public void ToJson_WithJSONActorWithANoneFilledChildRoleWhichIsAddedDuringExecution_ChildRoleIsCreatedUnderTheHoodAndSerializedWithTheChangedValues()
    {
        // Arrange
        var origin = JObject.FromObject(new
        {
            Name = "unittest",
        });
        var role = origin.Presents<IOrder>();
        
        // Create the "child" role
        role.Total = Acting.New<IMoney>();
        // Change properties of the "Child" role
        role.Total.Value = (decimal)100.10;
        
        // Act
        var jsn = Backlot.Core.Json.Json.ToJson(role, Strategy.SerializeForInteraction);
        
        // Assert
        Assert.Multiple(() =>
        {
            // Check if both, original actor and added child role properties are serialized.
            Assert.That(role.Name, Is.EqualTo("unittest"));
            Assert.That(jsn, Does.Contain("100.1"));
        });
    }
    
    [Test]
    public void ToJson_WithJSONActorHavingNullValueWhichIsSetByCodeAndUpdatedWithNewValue_NewValueSerialized()
    {
        // Arrange
        // var for this json {"Number1":1,"Number2":2,"Operation":"sum"}
        var origin = "{\"Number1\":1,\"Number2\":2,\"Operation\":null}";
        var role = origin.Presents<IFormula>();
        
        // update Null value
        role.Operation = "sum";
        
        // Act
        var json = Backlot.Core.Json.Json.ToJson(role, Strategy.SerializeForInteraction);
        
        // Assert
        Assert.That(json, Does.Contain("\"Operation\":\"sum\""));
    }
    
    [Test]
    public void ToJson_WithJSONActorWhereAPropertyValueIsChangedToNullAndSerializedForPersistence_TheNullValueIsRespectedAfterSerialization()
    {
        // Arrange
        var origin = JObject.FromObject(new
        {
            Number1 = 1,
            Number2 = 2,
            Operation = "sum",
            extra = "extras"
        });
        var role = origin.Presents<IFormula>();
        
        // update value to null
        role.Operation = null;
        
        // Act
        var json = role.ToJson(Strategy.SerializeForPersistance);
        var result = JObject.Parse(json);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result["extra"]?.Value<string>(), Is.EqualTo("extras"));
            Assert.That(result.ToString(), Does.Contain("Operation"));
            Assert.That(result["Operation"]?.Value<string>(), Is.Null);
        });
    }
}