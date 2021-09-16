using System.Collections.Generic;

namespace UIKit
{
    public class NavigationController
    {
        private readonly List<ViewController> _viewControllers = new List<ViewController>();
        private int _lastIndex => _viewControllers.Count - 1;

        public IReadOnlyList<ViewController> viewControllers => _viewControllers;
        public ViewController presentingViewController => _viewControllers.Count == 0 ? null : _viewControllers[_lastIndex];
        public bool isBeingPresented { get; private set; } = default;

        public NavigationController(ViewController viewController)
        {
            _viewControllers.Add(viewController);
            ToInterface(viewController).Configure(this);
        }

        public void Push(ViewController viewController, bool animated = true)
        {
            if (_viewControllers.Count > 0) _viewControllers[_lastIndex].Dismiss(animated);

            _viewControllers.Add(viewController);
            ToInterface(viewController).Configure(this);
            viewController.Present(animated);
        }

        public ViewController Pop(bool animated = true)
        {
            if (_viewControllers.Count == 0) return null;
            int lastIndex = _lastIndex;
            ViewController vc = _viewControllers[lastIndex];
            _viewControllers.RemoveAt(lastIndex);
            vc.Dismiss(animated);
            ToInterface(vc).Configure(null);

            if (_viewControllers.Count == 0) return vc;
            
            lastIndex -= 1;
            _viewControllers[lastIndex].Present(animated);

            return vc;
        }
        
        public void Present(bool animated = true)
        {
            if (_viewControllers.Count == 0) return;
            isBeingPresented = true;
            presentingViewController.Present(animated);            
        }

        public void Dismiss(bool animated = true)
        {
            isBeingPresented = false;
            if (_viewControllers.Count == 0) return;
            presentingViewController.Dismiss(animated);
        }

        private static IViewController ToInterface(IViewController viewController) => viewController;
    }
}
