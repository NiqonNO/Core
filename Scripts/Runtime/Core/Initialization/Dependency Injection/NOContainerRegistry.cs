using System;
using System.Collections.Generic;

namespace NiqonNO.Core
{
	public class NOContainerRegistry
	{
		private readonly Dictionary<string, NOContainer> ContainersByScope = new();
        private readonly Dictionary<string, List<IInitializable>> WaitingForInitialization = new();

        public void ResetState()
        {
            ContainersByScope.Clear();
            WaitingForInitialization.Clear();
        }

        public bool TryGetContainer(string scope, out NOContainer container)
        {
            return ContainersByScope.TryGetValue(scope ?? string.Empty, out container);
        }

        public void RegisterContainer(NOContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            ContainersByScope[container.MyScope] = container;
            if (!WaitingForInitialization.TryGetValue(container.MyScope, out var initializables)) return;

            foreach (var initializable in initializables)
            {
                InitializeInitializable(container, initializable);
            }
            WaitingForInitialization.Remove(container.MyScope);
        }

        public void UnregisterContainer(NOContainer container)
        {
            if (container == null) return;
            ContainersByScope.Remove(container.MyScope);
        }

        public void EnqueueForInitialization(string scope, IInitializable initializable)
        {
            if (initializable == null) return;
            scope ??= string.Empty;

            if (ContainersByScope.TryGetValue(scope, out var context))
            {
                InitializeInitializable(context, initializable);
                return;
            }

            if (!WaitingForInitialization.TryGetValue(scope, out var list))
            {
                list = new List<IInitializable>();
                WaitingForInitialization[scope] = list;
            }
            list.Add(initializable);
        }

        private void InitializeInitializable(NOContainer container, IInitializable initializable)
        {
            container.Inject(initializable);
            initializable.Initialize();
        }
	}
}