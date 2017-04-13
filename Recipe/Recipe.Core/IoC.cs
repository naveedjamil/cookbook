using Microsoft.Practices.Unity;

namespace Recipe.Core
{
    public static class IoC
    {
        private static IUnityContainer _container;

        static IoC()
        {
            _container = new UnityContainer();
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public static bool Exists<T>()
        {
            return _container.IsRegistered<T>();
        }

        public static IUnityContainer Container
        {
            get
            {
                return _container;
            }
        }
    }
}
