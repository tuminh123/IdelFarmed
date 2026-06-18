// FarmingInstaller.cs
using UnityEngine;
using Zenject;

public class FarmingInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        //// ScriptableObjects
        //Container.Bind<FarmingDatabaseSO>().FromInstance(_farmingDatabase).AsSingle();
        //Container.Bind<GameTimeConfigSO>().FromInstance(_gameTimeConfig).AsSingle();

        //// Repository — resolve sớm để load data
        //Container.Bind<FarmingRepository>().AsSingle();
        //var repo = Container.Resolve<FarmingRepository>();

        //// Load save data → bind instances
        //Container.BindInstance(repo.LoadUserData()).AsSingle();
        //Container.BindInstance(repo.LoadGameTime()).AsSingle();

        //// Currency
        //Container.Bind<FarmingCurrencyService>().AsSingle();
        //Container.Bind<ICurrencyService>().To<FarmingCurrencyService>().AsSingle();

        //// Grid
        //Container.BindInstance(new FarmingGrid(
        //    _farmingDatabase.GridWidth, _farmingDatabase.GridHeight)).AsSingle();

        //// Signals
        //Container.DeclareSignal<ZoneUnlockedSignal>();
        //Container.DeclareSignal<PlantPlantedSignal>();
        //Container.DeclareSignal<PlantReadySignal>();
        //Container.DeclareSignal<HarvestCompletedSignal>();
        //Container.DeclareSignal<DayStartedSignal>();
        //Container.DeclareSignal<NightStartedSignal>();
        //Container.DeclareSignal<TimeOfDayChangedSignal>();

        //// Services
        //Container.BindInterfacesAndSelfTo<GameTimeService>().AsSingle();
        //Container.Bind<ZoneManager>().AsSingle();
        //Container.BindInterfacesAndSelfTo<PlantManager>().AsSingle();

        //// FarmingService — NonLazy, load save trong constructor
        //Container.Bind<FarmingService>().AsSingle().NonLazy();
    }
}