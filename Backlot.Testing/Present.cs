using System.Collections.Generic;
using System.Threading.Tasks;
using Backlot.Core;
using Backlot.Core.Abstraction.Actors;
using Backlot.Core.Abstraction.Roles;
using Backlot.Core.DependencyInjection;
using Backlot.Core.Json;
using Backlot.Core.Services;
using Backlot.Testing.Core;
using Backlot.Testing.Shared.Scenarios;
using Backlot.Testing.Shared.TestModels;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
// ReSharper disable SuspiciousTypeConversion.Global

namespace Backlot.Testing;

/// <summary>
/// Tests on presenting a role from a source object or loaded from a database.
/// </summary>
public class Present
{
    [SetUp]
    public void Setup()
    {
        Initialize.Setup();
    }

    [Test]
    public async Task Presents_AnonymousActor_ProxiedRole()
    {
        #region ARRANGE

        string uid = "023d02600ded";

        var initial = new
        {
            Uid = uid,
            Number1 = 7,
            Number2 = 9,
            Operation = "sum"
        };

        var repo = ServiceLocator.Get<IPersistedRoleRepository>();

        #endregion

        #region ACT

        var formula = initial.Presents<IFormula>();
        await Dummy.Play(formula);
        repo.TryGet<IFormula>(uid, out var persisted);

        #endregion

        //the proxies are not of the same type as the actor, they present the role;
        Assert.That(formula is IProxiedRole);

        //the underlying actor is of the original type.
        var actor = (formula as IProxiedRole)!.Actor;
        Assert.That(actor != null);

        //when presenting the role, the role properties are accessable.
        Assert.That(formula.Number1 == 7);

        Assert.That(formula is IProxiedRole);

        var pactor = (persisted as IProxiedRole)!.Actor;
        Assert.That(pactor != null);

        //anonymous objects aren't supported bij repos anymore. We persist them as JObjects.
        Assert.That(pactor is JObject);

        Assert.That(persisted.Number1 == 7);
    }
    

}