using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Framework.BusinessObjectLayer.BusinessObjects
{
    public abstract class NotifyingObject : INotifyPropertyChanged, IDisposable, IComparable
    {
        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might
        /// override this property's getter to return true.
        /// </summary>
        private bool _ThrowOnInvalidPropertyName;
        private bool _CleanedUp;

        protected NotifyingObject()
        {
            this._CleanedUp = false;
        }

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This
        /// method does not exist in a Release build.
        /// </summary>
        [DebuggerStepThrough]
        [Conditional("DEBUG")]
        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties((object)this)[propertyName] != null)
                return;
            string message = "Invalid property name: " + propertyName;
            if (this.ThrowOnInvalidPropertyName)
                throw new Exception(message);
            Debug.Fail(message);
        }

        protected virtual bool ThrowOnInvalidPropertyName
        {
            get
            {
                return this._ThrowOnInvalidPropertyName;
            }
            set
            {
                this._ThrowOnInvalidPropertyName = value;
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event by determining property
        /// name using stack trace.
        /// </summary>
        protected virtual void OnPropertyChanged()
        {
            StackTrace stackTrace = new StackTrace();
            string name = stackTrace.GetFrame(1).GetMethod().Name;
            if (!stackTrace.GetFrame(1).GetMethod().Name.StartsWith("set_"))
                Debug.Fail("OnPropertyChanged-method without parameter was not called from a property setter.");
            this.OnPropertyChanged(name.Remove(0, 4));
        }

        /// <summary>Raised when a property on this object has a new value.</summary>
        public event PropertyChangedEventHandler PropertyChangedEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Raises this object's PropertyChanged event.</summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);
            PropertyChangedEventHandler propertyChangedEvent = this.PropertyChangedEvent;
            if (propertyChangedEvent == null)
                return;
            propertyChangedEvent((object)this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            this.CleanUp(true);
            this._CleanedUp = true;
            GC.SuppressFinalize((object)this);
        }

        /// <summary>
        /// Ensures that ViewModel gets properly garbage collected
        /// in case the dispose-method is not called by the client.
        /// Override CleanUpManagedResources-method instead of this one.
        /// </summary>
        ~NotifyingObject()
        {
            this.CleanUp(false);
            this._CleanedUp = true;
        }

        private void CleanUp(bool disposing)
        {
            if (this._CleanedUp)
                return;
            if (disposing)
                this.CleanUpManagedResources();
            this.CleanUpUnmanagedResources();
        }

        /// <summary>Override this method to clean up managed resources.</summary>
        protected virtual void CleanUpManagedResources()
        {
        }

        /// <summary>Override this method to clean up unmanaged resources.</summary>
        protected virtual void CleanUpUnmanagedResources()
        {
        }

        /// <summary>
        /// Compares current object with another one of same datatype
        /// using their overridable ToString-representation.
        /// </summary>
        /// <remarks>
        /// It is recommended to also override the Equals- and
        /// GetHashcode-methods to guarantee consistent results.
        /// </remarks>
        public virtual int CompareTo(object obj)
        {
            if (Operators.CompareString(obj.ToString(), this.ToString(), false) > 0)
                return 1;
            return Operators.CompareString(obj.ToString(), this.ToString(), false) < 0 ? -1 : 0;
        }
    }
}
