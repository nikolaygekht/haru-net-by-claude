/*
 * << Haru Free PDF Library >> -- HpdfChoice.cs
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
using System.Linq;
using Haru.Objects;
using Haru.Xref;

namespace Haru.Forms
{
    /// <summary>
    /// Choice field for list boxes and combo boxes (dropdowns).
    /// </summary>
    public class HpdfChoice : HpdfField
    {
        private readonly List<string> _options;

        /// <summary>
        /// Creates a new choice field.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="name">The field name.</param>
        /// <param name="isComboBox">True for combo box (dropdown), false for list box.</param>
        public HpdfChoice(HpdfXref xref, string name, bool isComboBox = true)
            : base(xref, HpdfFieldType.Ch, name)
        {
            _options = new List<string>();

            // Set the Combo flag if this is a combo box
            if (isComboBox)
                Flags = HpdfFieldFlags.Combo;
        }

        /// <summary>
        /// Adds an option to the choice field.
        /// </summary>
        /// <param name="option">The option text.</param>
        public void AddOption(string option)
        {
            if (string.IsNullOrEmpty(option))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Option cannot be null or empty");

            _options.Add(option);
            UpdateOptionsArray();
        }

        /// <summary>
        /// Adds multiple options to the choice field.
        /// </summary>
        /// <param name="options">The options to add.</param>
        public void AddOptions(params string[] options)
        {
            if (options == null || options.Length == 0)
                return;

            foreach (var option in options)
            {
                if (!string.IsNullOrEmpty(option))
                    _options.Add(option);
            }

            UpdateOptionsArray();
        }

        /// <summary>
        /// Clears all options from the choice field.
        /// </summary>
        public void ClearOptions()
        {
            _options.Clear();
            _dict.Remove("Opt");
        }

        /// <summary>
        /// Gets the list of options.
        /// </summary>
        public IReadOnlyList<string> Options => _options.AsReadOnly();

        /// <summary>
        /// Sets whether this is an editable combo box.
        /// Only applicable if this is a combo box (Combo flag is set).
        /// </summary>
        /// <param name="editable">True to allow custom text input, false for selection only.</param>
        public void SetEditable(bool editable)
        {
            // Only applicable to combo boxes
            if ((Flags & HpdfFieldFlags.Combo) == 0)
                throw new HpdfException(HpdfErrorCode.InvalidOperation,
                    "SetEditable is only applicable to combo boxes");

            if (editable)
                Flags |= HpdfFieldFlags.Edit;
            else
                Flags &= ~HpdfFieldFlags.Edit;
        }

        /// <summary>
        /// Sets whether options should be sorted alphabetically.
        /// </summary>
        /// <param name="sort">True to sort options, false otherwise.</param>
        public void SetSort(bool sort)
        {
            if (sort)
                Flags |= HpdfFieldFlags.Sort;
            else
                Flags &= ~HpdfFieldFlags.Sort;
        }

        /// <summary>
        /// Sets whether multiple selections are allowed.
        /// Only applicable to list boxes (not combo boxes).
        /// </summary>
        /// <param name="multiSelect">True to allow multiple selections, false for single selection.</param>
        public void SetMultiSelect(bool multiSelect)
        {
            // Only applicable to list boxes (not combo boxes)
            if ((Flags & HpdfFieldFlags.Combo) != 0)
                throw new HpdfException(HpdfErrorCode.InvalidOperation,
                    "SetMultiSelect is only applicable to list boxes (not combo boxes)");

            if (multiSelect)
                Flags |= HpdfFieldFlags.MultiSelect;
            else
                Flags &= ~HpdfFieldFlags.MultiSelect;
        }

        /// <summary>
        /// Sets the selected value(s) for this choice field.
        /// For single-selection fields, provide one value.
        /// For multi-selection list boxes, provide multiple values.
        /// </summary>
        /// <param name="values">The selected value(s).</param>
        public void SetSelectedValues(params string[] values)
        {
            if (values == null || values.Length == 0)
            {
                _dict.Remove("V");
                return;
            }

            // Validate that all values are in the options list
            foreach (var value in values)
            {
                if (!_options.Contains(value))
                    throw new HpdfException(HpdfErrorCode.InvalidParameter,
                        $"Value '{value}' is not in the options list");
            }

            _dict.Remove("V");
            if (values.Length == 1)
            {
                // Single value
                _dict.Add("V", new HpdfString(values[0]));
            }
            else
            {
                // Multiple values (only valid for multi-select list boxes)
                if ((Flags & HpdfFieldFlags.MultiSelect) == 0)
                    throw new HpdfException(HpdfErrorCode.InvalidOperation,
                        "Multiple values are only allowed for multi-select list boxes");

                var valueArray = new HpdfArray();
                foreach (var value in values)
                {
                    valueArray.Add(new HpdfString(value));
                }
                _dict.Add("V", valueArray);
            }
        }

        /// <summary>
        /// Sets the field value using a string.
        /// </summary>
        /// <param name="value">The selected value.</param>
        public override void SetValue(string value)
        {
            if (value == null)
            {
                _dict.Remove("V");
                return;
            }

            SetSelectedValues(value);
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

        /// <summary>
        /// Updates the /Opt array in the field dictionary.
        /// </summary>
        private void UpdateOptionsArray()
        {
            _dict.Remove("Opt");
            if (_options.Count > 0)
            {
                var optArray = new HpdfArray();
                foreach (var option in _options)
                {
                    optArray.Add(new HpdfString(option));
                }
                _dict.Add("Opt", optArray);
            }
        }
    }
}
