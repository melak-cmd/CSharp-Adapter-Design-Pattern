using System;
using System.Collections.Generic;
using System.Threading;

namespace Framework.BusinessObjectLayer.Validation
{
    public class Validation
    {
        public event ValidationChangedEventHandler ValidationChangedEvent;

        public delegate void ValidationChangedEventHandler();

        Dictionary<string, string> validationList;

        public int Count
        {
            get
            {
                return this.ValidationList.Count;
            }
        }


        public Dictionary<string, string> ValidationList
        {
            get
            {
                return validationList;
            }
            set
            {
                validationList = value;
            }
        }

        public Validation()
        {
            validationList = new Dictionary<string, string>();
        }

        public void ClearError(string propertyName)
        {
            if (this.ValidationList.ContainsKey(propertyName))
            {
                this.ValidationList.Remove(propertyName);
            }
            ValidationChangedEventHandler validationChangedEvent = this.ValidationChangedEvent;
            if (validationChangedEvent == null)
            {
                return;
            }
            validationChangedEvent();
        }

        public bool ValidateDateRange(string propertyName, DateTime value, DateTime min, DateTime max)
        {
            string empty = string.Empty;
            if (DateTime.Compare(value, min) >= 0 && DateTime.Compare(value, max) <= 0)
            {
                return true;
            }
            else
            {
                //NOOP
            }

            string errMessage = string.Format(Thread.CurrentThread.CurrentCulture, "Für das Feld '{3}' befindet sich das Datum {0} außerhalb der erlaubten Spanne von {1} bis {2}.", value.ToShortDateString(), min.ToShortDateString(), max.ToShortDateString(), propertyName);
            this.AddValidationError(propertyName, errMessage);
            return false;
        }

        public bool ValidateEmptyString(string propertyName, string value)
        {
            string empty = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                //NOOP
            }
            else
            {
                return true;
            }

            string errMessage = string.Format(Thread.CurrentThread.CurrentCulture, "Der Pflichteintrag für das Feld '{0}' fehlt.", new object[1] { propertyName });
            this.AddValidationError(propertyName, errMessage);
            return false;
        }

        protected internal void AddValidationError(string propertyName, string errMessage)
        {
            if (this.ValidationList.ContainsKey(propertyName))
            {
                if (this.ValidationList[propertyName].Contains(errMessage))
                {
                    //NOOP
                }
                else
                {
                    this.ValidationList[propertyName] = this.ValidationList[propertyName] + " " + errMessage;
                }
            }
            else
            {
                this.ValidationList.Add(propertyName, errMessage);
            }

            ValidationChangedEventHandler validationChangedEvent = this.ValidationChangedEvent;
            if (validationChangedEvent == null)
            {
                return;
            }

            validationChangedEvent();
        }
    }

    public class Validation<T> : Validation where T : BOBase
    {
    }
}