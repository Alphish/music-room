using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.MusicRoom.ViewModel
{
    /// <summary>
    /// Provides a base implementation of the INotifyPropertyChanged interface.
    /// Also, it includes a few helper methods for basic operations.
    /// </summary>
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises a PropertyChanged event for a given property.
        /// </summary>
        /// <param name="propertyName">The name of the property changed.</param>
        protected void Notify(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Changes the property value and notifies about the change, as long as the value provided is different.
        /// </summary>
        /// <typeparam name="TValue">The type of the value provided.</typeparam>
        /// <param name="propertyName">The name of the property changed.</param>
        /// <param name="value">The value to set the property to.</param>
        protected void Set<TValue>(string propertyName, TValue value)
        {
            FieldInfo backingField = GetType().GetField("_" + propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            var currentValue = (TValue)backingField.GetValue(this);
            if ((currentValue == null && value != null) || (currentValue != null && !currentValue.Equals(value)))
            {
                backingField.SetValue(this, value);
                Notify(propertyName);
            }
        }

        /// <summary>
        /// Changes the property value through an underlying object and notifies about the change, as long as the value provided is different.
        /// </summary>
        /// <typeparam name="TValue">The type of the value provided.</typeparam>
        /// <param name="innerObject">The underlying object the property depends on.</param>
        /// <param name="propertyName">The name of the property changed.</param>
        /// <param name="value">The value to set the property to.</param>
        protected void Set<TValue>(object innerObject, string propertyName, TValue value)
        {
            PropertyInfo innerProperty = innerObject.GetType().GetProperty(propertyName);
            var currentValue = (TValue)innerProperty.GetValue(innerObject);
            if (!currentValue.Equals(value))
            {
                innerProperty.SetValue(innerObject, value);
                Notify(propertyName);
            }
        }

    }
}
