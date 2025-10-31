/*
 * << Haru Free PDF Library >> -- HpdfField.cs
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
using Haru.Objects;
using Haru.Types;
using Haru.Xref;

namespace Haru.Forms
{
    /// <summary>
    /// Base class for all PDF form fields.
    /// Represents the data and structure of a form field.
    /// </summary>
    public abstract class HpdfField
    {
        private static readonly string[] FieldTypeNames = new[]
        {
            "Btn",  // Button
            "Tx",   // Text
            "Ch",   // Choice
            "Sig"   // Signature
        };

        protected readonly HpdfDict _dict;
        protected readonly HpdfXref _xref;
        protected readonly HpdfFieldType _fieldType;
        protected readonly List<HpdfWidgetAnnotation> _widgets;
        protected HpdfFieldFlags _flags;

        /// <summary>
        /// Gets the underlying dictionary object.
        /// </summary>
        public HpdfDict Dict => _dict;

        /// <summary>
        /// Gets the field type.
        /// </summary>
        public HpdfFieldType FieldType => _fieldType;

        /// <summary>
        /// Gets or sets the field flags.
        /// </summary>
        public HpdfFieldFlags Flags
        {
            get => _flags;
            set
            {
                _flags = value;
                _dict.Remove("Ff");
                if (_flags != HpdfFieldFlags.None)
                    _dict.Add("Ff", new HpdfNumber((int)_flags));
            }
        }

        /// <summary>
        /// Gets the list of widget annotations for this field.
        /// </summary>
        public IReadOnlyList<HpdfWidgetAnnotation> Widgets => _widgets.AsReadOnly();

        /// <summary>
        /// Creates a new form field.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="fieldType">The field type.</param>
        /// <param name="name">The partial field name.</param>
        protected HpdfField(HpdfXref xref, HpdfFieldType fieldType, string name)
        {
            _xref = xref ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");
            _fieldType = fieldType;
            _dict = new HpdfDict();
            _widgets = new List<HpdfWidgetAnnotation>();
            _flags = HpdfFieldFlags.None;

            // Add to xref
            _xref.Add(_dict);

            // Set field type
            _dict.Add("FT", new HpdfName(FieldTypeNames[(int)fieldType]));

            // Set field name
            if (!string.IsNullOrEmpty(name))
                _dict.Add("T", new HpdfString(name));
        }

        /// <summary>
        /// Sets the field value.
        /// </summary>
        /// <param name="value">The field value.</param>
        public virtual void SetValue(string value)
        {
            _dict.Remove("V");
            if (value != null)
            {
                _dict.Add("V", new HpdfString(value));
            }
        }

        /// <summary>
        /// Sets the default field value.
        /// </summary>
        /// <param name="value">The default value.</param>
        public virtual void SetDefaultValue(string value)
        {
            _dict.Remove("DV");
            if (value != null)
            {
                _dict.Add("DV", new HpdfString(value));
            }
        }

        /// <summary>
        /// Adds a widget annotation to this field.
        /// </summary>
        /// <param name="widget">The widget annotation.</param>
        public void AddWidget(HpdfWidgetAnnotation widget)
        {
            if (widget is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Widget cannot be null");

            _widgets.Add(widget);

            // Set up bidirectional linking
            widget.SetParent(this);

            // Update Kids array in field dictionary
            UpdateKidsArray();
        }

        /// <summary>
        /// Creates and adds a widget annotation to this field.
        /// </summary>
        /// <param name="rect">The widget rectangle.</param>
        /// <returns>The created widget annotation.</returns>
        public HpdfWidgetAnnotation CreateWidget(HpdfRect rect)
        {
            var widget = new HpdfWidgetAnnotation(_xref, rect);
            AddWidget(widget);
            return widget;
        }

        /// <summary>
        /// Sets the default appearance string for this field.
        /// Format: /FontName FontSize Tf R G B rg
        /// Example: /Helv 12 Tf 0 0 0 rg
        /// </summary>
        /// <param name="appearance">The default appearance string.</param>
        public void SetDefaultAppearance(string appearance)
        {
            _dict.Remove("DA");
            if (!string.IsNullOrEmpty(appearance))
            {
                _dict.Add("DA", new HpdfString(appearance));
            }
        }

        /// <summary>
        /// Updates the Kids array in the field dictionary.
        /// </summary>
        private void UpdateKidsArray()
        {
            _dict.Remove("Kids");
            if (_widgets.Count > 0)
            {
                var kidsArray = new HpdfArray();
                foreach (var widget in _widgets)
                {
                    kidsArray.Add(widget.Dict);
                }
                _dict.Add("Kids", kidsArray);
            }
        }
    }
}
