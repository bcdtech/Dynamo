using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Dynamo.Core
{

    /// <summary>
    /// This class notifies the View when there is a change.    
    /// </summary>
    [Serializable]
    public abstract class NotificationObject : INotifyPropertyChanged
    {
        internal PropertyChangeManager PropertyChangeManager { get; } = new PropertyChangeManager();
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>        
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            //TODO profile this?
            if (!PropertyChangeManager.ShouldRaiseNotification(propertyName))
            {
                return;
            }
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
