/*
 * << Haru Free PDF Library >> -- HpdfRadioButton.cs
 *
 * C# port of Haru Free PDF Library
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 * provided that the above copyright notice appear in all copies and
 * that both that copyright notice and this permission notice appear
 * in supporting documentation.
 * It is provided "as is" without express or implied warranty.
 *
 */

using System.Collections.Generic;
using Haru.Forms.Appearance;
using Haru.Objects;
using Haru.Types;
using Haru.Xref;

namespace Haru.Forms
{
    /// <summary>
    /// Radio button field where exactly one option can be selected from a group.
    /// Each radio button group has multiple widgets, each representing one option.
    /// </summary>
    public class HpdfRadioButton : HpdfField
    {
        private const string UncheckedState = "Off";
        private readonly Dictionary<string, HpdfWidgetAnnotation> _optionWidgets;

        /// <summary>
        /// Creates a new radio button field.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="name">The field name (name of the radio button group).</param>
        public HpdfRadioButton(HpdfXref xref, string name)
            : base(xref, HpdfFieldType.Btn, name)
        {
            _optionWidgets = new Dictionary<string, HpdfWidgetAnnotation>();

            // Set the Radio flag
            Flags = HpdfFieldFlags.Radio;

            // Default value is Off (nothing selected)
            SetValue(UncheckedState);
        }

        /// <summary>
        /// Adds a radio button option with a specific state name.
        /// Each option represents one choice in the radio button group.
        /// </summary>
        /// <param name="stateName">The state name for this option (e.g., "Male", "Female", "Option1").</param>
        /// <param name="widget">The widget annotation for this option.</param>
        public void AddOption(string stateName, HpdfWidgetAnnotation widget)
        {
            if (string.IsNullOrEmpty(stateName))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "State name cannot be null or empty");

            if (widget is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Widget cannot be null");

            if (stateName == UncheckedState)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "State name cannot be 'Off' (reserved)");

            _optionWidgets[stateName] = widget;
            AddWidget(widget);

            // Set initial appearance state to Off
            widget.SetAppearanceState(UncheckedState);
        }

        /// <summary>
        /// Creates and adds a radio button option.
        /// </summary>
        /// <param name="stateName">The state name for this option.</param>
        /// <param name="rect">The widget rectangle.</param>
        /// <returns>The created widget annotation.</returns>
        public HpdfWidgetAnnotation CreateOption(string stateName, HpdfRect rect)
        {
            var widget = new HpdfWidgetAnnotation(_xref, rect);

            // Set up appearance streams for radio button (stateName/Off states) with actual drawing commands
            widget.SetButtonAppearances(ButtonElementType.RadioButton, stateName, UncheckedState);

            AddOption(stateName, widget);
            return widget;
        }

        /// <summary>
        /// Sets the selected radio button option.
        /// </summary>
        /// <param name="stateName">The state name of the option to select, or "Off" for none.</param>
        public void SetSelected(string stateName)
        {
            SetValue(stateName);

            // Update appearance states for all widgets
            foreach (var kvp in _optionWidgets)
            {
                kvp.Value.SetAppearanceState(kvp.Key == stateName ? stateName : UncheckedState);
            }
        }

        /// <summary>
        /// Sets the field value using a string.
        /// </summary>
        /// <param name="value">The state name to select.</param>
        public override void SetValue(string value)
        {
            // Allow null to mean no selection
            if (value is null)
                value = UncheckedState;

            // Validate that the value is either Off or one of the registered options
            if (value != UncheckedState && !_optionWidgets.ContainsKey(value))
                throw new HpdfException(HpdfErrorCode.InvalidParameter,
                    $"Invalid radio button value: {value}. Must be 'Off' or a registered option.");

            _dict.Remove("V");
            _dict.Add("V", new HpdfName(value));
        }

        /// <summary>
        /// Sets the default selected option.
        /// </summary>
        /// <param name="stateName">The state name of the default option, or "Off" for none.</param>
        public void SetDefaultSelected(string stateName)
        {
            if (stateName is null)
                stateName = UncheckedState;

            if (stateName != UncheckedState && !_optionWidgets.ContainsKey(stateName))
                throw new HpdfException(HpdfErrorCode.InvalidParameter,
                    $"Invalid radio button value: {stateName}. Must be 'Off' or a registered option.");

            _dict.Remove("DV");
            _dict.Add("DV", new HpdfName(stateName));
        }

        /// <summary>
        /// Sets whether this field is read-only.
        /// </summary>
        /// <param name="readOnly">True for read-only, false for editable.</param>
        public void SetReadOnly(bool readOnly)
        {
            if (readOnly)
                Flags |= HpdfFieldFlags.ReadOnly;
            else
                Flags &= ~HpdfFieldFlags.ReadOnly;
        }

        /// <summary>
        /// Sets whether this field is required.
        /// </summary>
        /// <param name="required">True if the field must have a value.</param>
        public void SetRequired(bool required)
        {
            if (required)
                Flags |= HpdfFieldFlags.Required;
            else
                Flags &= ~HpdfFieldFlags.Required;
        }
    }
}
