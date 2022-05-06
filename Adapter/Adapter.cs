using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Adapter
{

    /// <summary>
    /// Enables conversion from registered source type to a new instance of the registered target type.
    /// The new target type instance is initialized with a copy of all property values from the source
    /// type as defined in the data contract.
    /// </summary>
    /// <remarks>Both source and target type must implement the registered data contract.
    /// If a property value it-self is also a registered source type, the property value will also
    /// be converted into the registered target type following the same conversion rules.
    /// If a source type to convert contains a generic list or dictionary collection containing 
    /// registered source types, the adapter will convert all it collection items to the target type.
    /// </remarks>
    public class Adapter
    {

        private readonly Dictionary<Type, Type> _DataContracts = new Dictionary<Type, Type>();

        private readonly Dictionary<Type, Type> _TargetTypes = new Dictionary<Type, Type>();

        /// <summary>
        /// Converts an instance of a registered source type into a new instance
        /// of the registered target type by copying the values defined in the
        /// common data contract.
        /// </summary>
        /// <param name="source">Object instance to convert.</param>
        /// <returns>Converted object instance.</returns>
        /// <remarks>When trying to convert a non registered datatype, an 
        /// exception is thrown.</remarks>
        public virtual object Convert(object source)
        {

            Type dataContract = default(Type);
            Type targetType = default(Type);

            this._DataContracts.TryGetValue(source.GetType(), out dataContract);
            this._TargetTypes.TryGetValue(source.GetType(), out targetType);

            if (dataContract == null || targetType == null)
            {
                throw new Exception(string.Format("Source type to convert is not registered."));
            }

            dynamic newObject = Activator.CreateInstance(targetType);

            // Copy property values as defined in the data contract
            dynamic dataProperties = dataContract.GetProperties();

            foreach (var dataProperty in dataProperties)
            {

                // Assert if dataProperty it-self needs to be converted
                if (this._TargetTypes.ContainsKey(dataProperty.PropertyType))
                {
                    // Copy values of do/bo property by conversion
                    dynamic sourceProperty = dataProperty.GetValue(source, null);
                    dynamic targetProperty = this.Convert(sourceProperty);

                    dataProperty.SetValue(newObject, targetProperty, null);

                    // Property is not a registered source type but could be a list collection 
                    // containing registerd types to convert
                }
                else if ((dataProperty.PropertyType.GetInterface("IList`1") != null))
                {
                    // Property to copy is a generic list
                    // Find out if sourceItemType needs to be converted by looking for the datacontract
                    dynamic sourceList = (IList)dataProperty.GetValue(source, null);
                    dynamic targetListPropertyInfo = newObject.GetType().GetProperty(dataProperty.Name);
                    dynamic targetList = (IList)targetListPropertyInfo.GetValue(newObject, null);

                    if (sourceList.Count > 0)
                    {
                        if (this._TargetTypes.ContainsKey(sourceList[0].GetType()))
                        {
                            // sourceItem is a registered type and must be converted
                            foreach (var sourceItem in sourceList)
                            {
                                dynamic newO = this.Convert(sourceItem);
                                targetList.Add(newO);
                            }
                        }
                        else
                        {
                            // Copy list as a whole
                            dataProperty.SetValue(newObject, dataProperty.GetValue(source, null), null);
                        }
                    }
                }
                else if ((dataProperty.PropertyType.GetInterface("IDictionary`2") != null))
                {
                    // Property to copy is a generic dictionary
                    // Find out if sourceItemType needs to be converted by looking for a registerd data contract
                    dynamic sourceList = (IDictionary)dataProperty.GetValue(source, null);
                    dynamic targetListPropertyInfo = newObject.GetType().GetProperty(dataProperty.Name);
                    dynamic targetList = (IDictionary)targetListPropertyInfo.GetValue(newObject, null);

                    if (sourceList.Count > 0)
                    {
                        dynamic gen = sourceList.GetType().GetInterface("IDictionary`2").GetGenericArguments();
                        
                        if (this._DataContracts.ContainsValue(gen[0]) || this._DataContracts.ContainsValue(gen[1]))
                        {
                            dynamic mustConvertKey = this._DataContracts.ContainsValue(gen[0]);
                            dynamic mustConvertValue = this._DataContracts.ContainsValue(gen[1]);
                            object key = null;
                            object value = null;

                            foreach (var sourceItem in sourceList)
                            {
                                //Get key value
                                if (mustConvertKey)
                                {
                                    key = this.Convert(sourceItem.Key);
                                }
                                else
                                {
                                    key = sourceItem.Key;
                                }

                                // Get 'value' value
                                if (mustConvertValue)
                                {
                                    value = this.Convert(sourceItem.Value);
                                }
                                else
                                {
                                    value = sourceItem.Value;
                                }

                                ((IDictionary)targetList).Add(key, value);
                            }
                        }
                        else
                        {
                            // Copy dictionary as a whole
                            dataProperty.SetValue(newObject, dataProperty.GetValue(source, null), null);
                        }
                    }
                }
                else
                {
                    // Copy simple property
                    dataProperty.SetValue(newObject, dataProperty.GetValue(source, null), null);
                }
            }

            return newObject;
        }
        /// <summary>
        /// Registers a source type for conversion.
        /// </summary>
        /// <param name="dataContract">Data contract to implement by both source type and target type.</param>
        /// <param name="sourceType">Type to convert from.</param>
        /// <param name="targetType">Type to convert to.</param>
        /// <remarks>Data contract defines the properties to be copied from source to target.</remarks>

        public void Register(Type dataContract, Type sourceType, Type targetType)
        {
            if (_DataContracts.ContainsKey(sourceType) || _TargetTypes.ContainsKey(sourceType))
            {
                throw new Exception(string.Format("Source type {0} is already registered.", sourceType.Name));
            }

            if (sourceType.GetInterface(dataContract.Name) == null || targetType.GetInterface(dataContract.Name) == null)
            {
                throw new Exception(string.Format("Either source type, target type or both don't implement the common data contract."));
            }

            _DataContracts.Add(sourceType, dataContract);
            _TargetTypes.Add(sourceType, targetType);
        }

        /// <summary>
        /// Unregisters a source type for conversion.
        /// </summary>
        /// <param name="sourceType">Type to unregister from conversion list.</param>
        public void Unregister(Type sourceType)
        {
            if (_DataContracts.ContainsKey(sourceType))
            {
                _DataContracts.Remove(sourceType);
            }
            if (_TargetTypes.ContainsKey(sourceType))
            {
                _TargetTypes.Remove(sourceType);
            }
        }
    }

}