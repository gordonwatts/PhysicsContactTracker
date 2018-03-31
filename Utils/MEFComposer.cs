using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Text;

namespace Utils
{
    /// <summary>
    /// Static class to help with MEF composition
    /// (for testing and other things)
    /// </summary>
    public static class MEFComposer
    {
        /// <summary>
        /// The lightweight container that holds onto our parts
        /// </summary>
        private static Lazy<ContainerConfiguration> _containerConfig = null;

        /// <summary>
        /// Reset our container. Used only in testing.
        /// </summary>
        public static void Reset()
        {
            _containerConfig = new Lazy<ContainerConfiguration>(() => new ContainerConfiguration());
            _host = new Lazy<CompositionHost>(() => _containerConfig.Value.CreateContainer());
        }

        /// <summary>
        /// Setup the static variables
        /// </summary>
        /// <remarks>
        /// Do this so we don't have to redo the Lazy code.
        /// </remarks>
        static MEFComposer()
        {
            Reset();
        }

        /// <summary>
        /// Add an object type to the continer we are using to MEF things up with.
        /// </summary>
        /// <param name="partType"></param>
        public static void AddObject(Type partType)
        {
            if (_host.IsValueCreated)
            {
                throw new InvalidOperationException("Cannot add new part when MEF host has already been created.");
            }

            _containerConfig.Value.WithPart(partType);
        }

        /// <summary>
        /// Hold onto the resolution dude.
        /// </summary>
        private static Lazy<CompositionHost> _host = null;

        /// <summary>
        /// Run MEF on an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void Resolve<T>(T obj)
        {
            _host.Value.SatisfyImports(obj);
        }
    }
}
