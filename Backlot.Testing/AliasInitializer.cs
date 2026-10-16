using System.Collections.Generic;
using Backlot.Core.Abstraction.Actors;
using Backlot.Core.Json;
using Backlot.Core.Services;
using Backlot.Services.Filesystem.LocalDiskStorage;
using Backlot.Testing.Core;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Dynamic;
using System.Linq;
using Backlot.Core.Json.Serialization.Newtonsoft;
using Backlot.Testing.Shared.TestModels;
using NSubstitute;

namespace Backlot.Testing;

public class AliasInitializer
{
    [SetUp]
    public void Setup()
    {
        Initialize.Setup((builder) => new AliasDirector(new LocalDiskStorage(), Substitute.For<IConfigurationManager>(), builder));
    }
    
    // =================================================== JSON =============================================================
    
    [Test]
    public void Serialize_WithJSONActorUsingAnAliasNamedProperty_RolePropertyNameUsedActorNameRemovedInSerialization()
    {
        #region ARRANGE
        
        var origin = JObject.FromObject(new
        {
            Number1 = 7,
            Number2 = 9,
            Number3 = 4,
            Op = "sum",
        });
        
        #endregion

        #region ACT

        var role = origin.Presents<IFormula>();
        role.Operation = "new";
        
        var jsn = Backlot.Core.Json.Json.ToJson(role, Strategy.SerializeForInteraction);
        
        #endregion

        Assert.That(jsn.Contains("\"Op\""), Is.False);
        Assert.That(jsn.Contains("\"Operation\""));
    }
    
    [Test]
    public void Serialize_WithJSONActorUsingAnAliasMarkedPropertyWithoutAnAliasName_NormalFieldNameUsedInSerialization()
    {
        #region ARRANGE
        
        var origin = JObject.FromObject(new
        {
            Number1 = 7,
            Number2 = 9,
            Number3 = 4,
            Operation = "sum",
        });
        
        #endregion

        #region ACT

        var role = origin.Presents<IFormula>();
        role.Operation = "new";
        
        var jsn = Backlot.Core.Json.Json.ToJson(role, Strategy.SerializeForInteraction);
        
        #endregion

        Assert.That(jsn.Contains("\"Op\""), Is.False);
        Assert.That(jsn.Contains("\"Operation\""));
    }
    
    [Test]
    public void Serialize_WithTypedActorUsingAnAliasNamedProperty_RolePropertyNameUsedActorNameRemovedInSerialization()
    {
        #region ARRANGE
        
        var origin = new FormulaAlias
        {
            Number1 = 7,
            Number2 = 9,
            Op = "sum", // the actor name
        };
        
        #endregion

        #region ACT

        var role = origin.Presents<IFormula>();
        role.Operation = "new";
        
        var jsn = Backlot.Core.Json.Json.ToJson(role, Strategy.SerializeForInteraction);
        
        #endregion

        Assert.That(JObject.Parse(jsn)["Op"] == null); // actor name removed from root
        Assert.That(JObject.Parse(jsn)["Operation"] != null);
    }
    
    [Test]
    public void Serialize_WithTypedActorUsingAnAliasMarkedPropertyWithoutAnAliasName_ActorNameUsedRoleDefinedNameIgnoredInSerialization()
    {
        #region ARRANGE
        
        var origin = new Formula()
        {
            Number1 = 7,
            Number2 = 9,
            Operation = "sum",
        };
        
        #endregion

        #region ACT

        var role = origin.Presents<IFormula>();
        role.Operation = "new";
        
        var jsn = Backlot.Core.Json.Json.ToJson(role, Strategy.SerializeForInteraction);
        
        #endregion

        Assert.That(jsn.Contains("\"Op\""), Is.False);
        Assert.That(jsn.Contains("\"Operation\""));
    }
    
    [Test]
    public void Serialize_WithJSONActorUsingAnAliasDefinedPropertyUsinFieldInfoAlias_RolePropertyNameUsedActorNameRemovedInSerialization()
    {
        #region ARRANGE
        
        var origin = JObject.FromObject(new
        {
            Id = "023d02600ded",
            Number1 = 7,
            Number2 = 9,
            Number3 = 4,
            Operation = "sum",
        });
        
        #endregion

        #region ACT

        var role = origin.Presents<IFormula>();
        
        var jsn = Backlot.Core.Json.Json.ToJson(role, Strategy.SerializeForInteraction);
        
        #endregion

        Assert.That(role.Uid == "023d02600ded");
        Assert.That(jsn.Contains("\"Id\""), Is.False); // is NOT serialized because its used by Uid
        Assert.That(jsn.Contains("\"Uid\""));
        
    }
    
    [Test]
    public void Serialize_WithJSONActorHavingTheOriginalPropertyAndAnAliasPropertyWithSerializeForPersistenceStrategy_RoleIsUsingTheOriginalAsDefault()
    {
        #region ARRANGE
        
        var origin = JObject.FromObject(new
        {
            Uid = "123", // default
            Id = "023d02600ded", // skipped but serialized
            Number1 = 7,
            Number2 = 9,
            Number3 = 4,
            Operation = "sum",
        });
        
        #endregion

        #region ACT

        var role = origin.Presents<IFormula>();
        
        var jsn = Backlot.Core.Json.Json.ToJson(role, Strategy.SerializeForPersistance);
        
        #endregion

        Assert.That(role.Uid == "123");
        Assert.That(JObject.Parse(jsn)["Id"] != null); // is also serialized, because its skipped during aliassing and there for NOT used by Uid.
        Assert.That(JObject.Parse(jsn)["Uid"] != null);
        
    }
}