using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AdColony {

    // -------------------------------------------------------------------------
    // Base Options Class
    // -------------------------------------------------------------------------
    public class Options {
        /// <summary>
        /// Represents an AdColonyUserMetadata object.
        /// Configure and set this property to improve ad targeting.
        /// <see cref="AdColonyUserMetadata" />
        /// </summary>
        public UserMetadata Metadata;

        /// <summary>
        /// Sets a supported option.
        /// Use this method to set a string-based option with an arbitrary, string-based value.
        /// </summary>
        /// <param name="value">Value of the option.</param>
        /// <param name="key"> A string used to configure the option. Strings must be 128 characters or less.</param>
        /// <see cref="AdColonyAppOptions" />
        /// <see cref="AdColonyAdOptions" />
        public void SetOption(string key, string value) {
            if (key == null) {
                Debug.Log("Invalid option key.");
                return;
            }

            if (value == null) {
                Debug.Log("Invalid option value.");
                return;
            }

            _data[key] = value;
        }

        /// <summary>
        /// Sets a supported option.
        /// Use this method to set a string-based option with an arbitrary, numerial value.
        /// </summary>
        /// <param name="value">Value of the option.</param>
        /// <param name="key"> A string used to configure the option. Strings must be 128 characters or less.</param>
        /// <see cref="AdColonyAppOptions" />
        /// <see cref="AdColonyAdOptions" />
        public void SetOption(string key, int value) {
            if (key == null) {
                Debug.Log("Invalid option key.");
                return;
            }

            _data[key] = value;
        }

        /// <summary>
        /// Sets a supported option.
        /// Use this method to set a string-based option with an arbitrary, numerial value.
        /// </summary>
        /// <param name="value">Value of the option.</param>
        /// <param name="key"> A string used to configure the option. Strings must be 128 characters or less.</param>
        /// <see cref="AdColonyAppOptions" />
        /// <see cref="AdColonyAdOptions" />
        public void SetOption(string key, double value) {
            if (key == null) {
                Debug.Log("Invalid option key.");
                return;
            }

            _data[key] = value;
        }

        /// <summary>
        /// Sets a supported option.
        /// Use this method to set a string-based option with an arbitrary, boolean value.
        /// </summary>
        /// <param name="value">Value of the option.</param>
        /// <param name="key"> A string used to configure the option. Strings must be 128 characters or less.</param>
        /// <see cref="AdColonyAppOptions" />
        /// <see cref="AdColonyAdOptions" />
        public void SetOption(string key, bool value) {
            if (key == null) {
                Debug.Log("Invalid option key.");
                return;
            }

            _data[key] = value;
        }

        /// <summary>
        /// Returns the string-based value associated with the given key.
        /// Call this method to obtain the string-based value associated with the given string-based key.
        /// </summary>
        /// <param name="key"> A string used to configure the option. Strings must be 128 characters or less.</param>
        /// <returns>The string-based value associated with the given key. Returns `null` if the option has not been set.</returns>
        /// <see cref="AdColonyAppOptions" />
        /// <see cref="AdColonyAdOptions" />
        public string GetStringOption(string key) {
            return _data.ContainsKey(key) ? _data[key] as string : null;
        }

        /// <summary>
        /// Returns the integer-based value associated with the given key.
        /// Call this method to obtain the integer-based value associated with the given string-based key.
        /// </summary>
        /// <param name="key"> A string used to configure the option. Strings must be 128 characters or less.</param>
        /// <returns>The integer-based value associated with the given key. Returns `null` if the option has not been set.</returns>
        /// <see cref="AdColonyAppOptions" />
        /// <see cref="AdColonyAdOptions" />
        public int GetIntOption(string key) {
            return _data.ContainsKey(key) ? Convert.ToInt32(_data[key]) : 0;
        }

        /// <summary>
        /// Returns the double-precision-based value associated with the given key.
        /// Call this method to obtain the double-precision-based value associated with the given string-based key.
        /// </summary>
        /// <param name="key"> A string used to configure the option. Strings must be 128 characters or less.</param>
        /// <returns>The double-precision-based value associated with the given key. Returns `null` if the option has not been set.</returns>
        /// <see cref="AdColonyAppOptions" />
        /// <see cref="AdColonyAdOptions" />
        public double GetDoubleOption(string key) {
            return _data.ContainsKey(key) ? Convert.ToDouble(_data[key]) : 0.0;
        }

        /// <summary>
        /// Returns the boolean-based value associated with the given key.
        /// Call this method to obtain the boolean-based value associated with the given string-based key.
        /// </summary>
        /// <param name="key"> A string used to configure the option. Strings must be 128 characters or less.</param>
        /// <returns>The boolean-based value associated with the given key. Returns `null` if the option has not been set.</returns>
        /// <see cref="AdColonyAppOptions" />
        /// <see cref="AdColonyAdOptions" />
        public bool GetBoolOption(string key) {
            return _data.ContainsKey(key) ? Convert.ToBoolean(_data[key]) : false;
        }

#region Internal Methods - do not call these

        protected Hashtable _data = new Hashtable();

        public Options() {
        }

        public Options(Hashtable values) {
            _data = new Hashtable(values);

            if (values.ContainsKey(Constants.OptionsMetadataKey)) {
                Hashtable metadataValues = values[Constants.OptionsMetadataKey] as Hashtable;
                Metadata = new UserMetadata(metadataValues);
                _data.Remove(Constants.OptionsMetadataKey);
            }
        }

        public Hashtable ToHashtable() {
            Hashtable data = new Hashtable(_data);
            if (Metadata != null) {
                Hashtable metadataData = Metadata.ToHashtable();
                data.Add(Constants.OptionsMetadataKey, metadataData);
            }
            return data;
        }

        public string ToJsonString() {
            Hashtable data = ToHashtable();
            return AdColonyJson.Encode(data);
        }

#endregion
    }

    // -------------------------------------------------------------------------
    // Application Options Class
    // -------------------------------------------------------------------------
    public class AppOptions : Options {
        private bool _disableLogging;
        /// <summary>
        /// Disables AdColony logging.
        /// AdColony logging is enabled by default.
        /// Set this property before calling Config() with a corresponding value of `YES` to disable AdColony logging.
        /// </summary>
        public bool DisableLogging {
            get {
                return _disableLogging;
            }
            set {
                _disableLogging = value;
                _data[Constants.AppOptionsDisableLoggingKey] = _disableLogging;
            }
        }

        private string _userId;
        /// <summary>
        /// Sets a custom identifier for the current user.
        /// Set this property to configure a custom identifier for the current user.
        /// Corresponding value must be 128 characters or less.
        /// </summary>
        public string UserId {
            get {
                return _userId;
            }
            set {
                _userId = value;
                _data[Constants.AppOptionsUserIdKey] = _userId;
            }
        }

        private AdOrientationType _adOrientation = AdOrientationType.AdColonyOrientationAll;
        /// <summary>
        /// Sets the desired ad orientation.
        /// Set this property to configure the desired orientation for your ads.
        /// </summary>
        /// <see creg="ADCOrientation" />
        public AdOrientationType AdOrientation {
            get {
                return _adOrientation;
            }
            set {
                _adOrientation = value;
                _data[Constants.AppOptionsOrientationKey] = Convert.ToInt32(_adOrientation);
            }
        }

        private bool _multiWindowEnabled;
        /// <summary>
        /// Used to alert AdColony that multi-window is enabled for your app, allowing us to adjust
        /// our interstitial layout as necessary.
        /// NOTE: Android only
        /// </summary>
        public bool MultiWindowEnabled {
            get {
                return _multiWindowEnabled;
            }
            set {
                _multiWindowEnabled = value;
                _data[Constants.AppOptionsMultiWindowEnabledKey] = _multiWindowEnabled;
            }
        }

        private string _originStore;
        /// <summary>
        /// Optionally set the origin store for this app (default: 'google').
        /// NOTE: Android only
        /// </summary>
        public string OriginStore {
            get {
                return _originStore;
            }
            set {
                _originStore = value;
                _data[Constants.AppOptionsOriginStoreKey] = _originStore;
            }
        }

        private bool _testModeEnabled;
        /// <summary>
        /// Force test ads, used for debugging only.
        /// </summary>
        public bool TestModeEnabled {
            get {
                return _testModeEnabled;
            }
            set {
                _testModeEnabled = value;
                _data[Constants.AppOptionsTestModeKey] = _testModeEnabled;
            }
        }

#region Internal Methods - do not call these

        public AppOptions() {

        }

        public AppOptions(Hashtable values) : base(values) {
            if (values != null) {
                _data = new Hashtable(values);

                if (values.ContainsKey(Constants.AppOptionsDisableLoggingKey)) {
                    _disableLogging = Convert.ToBoolean(values[Constants.AppOptionsDisableLoggingKey]);
                }
                if (values.ContainsKey(Constants.AppOptionsUserIdKey)) {
                    _userId = values[Constants.AppOptionsUserIdKey] as string;
                }
                if (values.ContainsKey(Constants.AppOptionsOrientationKey)) {
                    _adOrientation = (AdOrientationType)Convert.ToInt32(values[Constants.AppOptionsOrientationKey]);
                }
                if (values.ContainsKey(Constants.AppOptionsMultiWindowEnabledKey)) {
                    _multiWindowEnabled = Convert.ToBoolean(values[Constants.AppOptionsMultiWindowEnabledKey]);
                }
                if (values.ContainsKey(Constants.AppOptionsOriginStoreKey)) {
                    _originStore = values[Constants.AppOptionsOriginStoreKey] as string;
                }
            }
        }

#endregion
    }

    // -------------------------------------------------------------------------
    // Ad Specific Options Class
    // -------------------------------------------------------------------------
    public class AdOptions : Options {
        private bool _showPrePopup;
        /// <summary>
        /// Enables reward dialogs to be shown before an advertisement.
        /// These popups are disabled by default.
        /// Set this property with a corresponding value of `YES` to enable.
        /// </summary>
        public bool ShowPrePopup {
            get {
                return _showPrePopup;
            }
            set {
                _showPrePopup = value;
                _data[Constants.AdOptionsPrePopupKey] = _showPrePopup;
            }
        }

        /// <summary>
        /// Enables reward dialogs to be shown after an advertisement.
        /// These popups are disabled by default.
        /// Set this property with a corresponding value of `YES` to enable.
        /// </summary>
        private bool _showPostPopup;
        public bool ShowPostPopup {
            get {
                return _showPostPopup;
            }
            set {
                _showPostPopup = value;
                _data[Constants.AdOptionsPostPopupKey] = _showPostPopup;
            }
        }

#region Internal Methods - do not call these

        public AdOptions() {
        }

        public AdOptions(Hashtable values) : base(values) {
            if (values != null) {
                _data = new Hashtable(values);

                if (values.ContainsKey(Constants.AdOptionsPrePopupKey)) {
                    _showPrePopup = Convert.ToBoolean(values[Constants.AdOptionsPrePopupKey]);
                }
                if (values.ContainsKey(Constants.AdOptionsPostPopupKey)) {
                    _showPostPopup = Convert.ToBoolean(values[Constants.AdOptionsPostPopupKey]);
                }
            }
        }

#endregion
    }
}
