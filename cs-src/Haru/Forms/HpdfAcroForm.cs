/*
 * << Haru Free PDF Library >> -- HpdfAcroForm.cs
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
using Haru.Xref;

namespace Haru.Forms
{
    /// <summary>
    /// Document-level AcroForm container.
    /// Manages all form fields in a PDF document.
    /// </summary>
    public class HpdfAcroForm
    {
        private readonly HpdfDict _dict;
        private readonly HpdfXref _xref;
        private readonly List<HpdfField> _fields;
        private string _defaultAppearance;
        private bool _needAppearances;

        /// <summary>
        /// Gets the underlying dictionary object.
        /// </summary>
        public HpdfDict Dict => _dict;

        /// <summary>
        /// Gets the list of fields in this form.
        /// </summary>
        public IReadOnlyList<HpdfField> Fields => _fields.AsReadOnly();

        /// <summary>
        /// Creates a new AcroForm.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        public HpdfAcroForm(HpdfXref xref)
        {
            _xref = xref ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");
            _dict = new HpdfDict();
            _fields = new List<HpdfField>();
            _needAppearances = false;

            // Add to xref
            _xref.Add(_dict);

            // Set default appearance (Helvetica 12pt, black text)
            SetDefaultAppearance("/Helv 12 Tf 0 0 0 rg");
        }

        /// <summary>
        /// Sets the default appearance for form fields.
        /// This defines the default font and color for text fields.
        /// </summary>
        /// <param name="appearance">The default appearance string (e.g., "/Helv 12 Tf 0 0 0 rg").</param>
        public void SetDefaultAppearance(string appearance)
        {
            _defaultAppearance = appearance;
            _dict.Remove("DA");
            if (!string.IsNullOrEmpty(appearance))
            {
                _dict.Add("DA", new HpdfString(appearance));
            }
        }

        /// <summary>
        /// Gets the default appearance string.
        /// </summary>
        public string DefaultAppearance => _defaultAppearance;

        /// <summary>
        /// Sets whether PDF viewers should generate field appearances.
        /// If true, the viewer will generate appearances for fields.
        /// If false, the document must provide appearances.
        /// </summary>
        /// <param name="needAppearances">Whether appearances need to be generated.</param>
        public void SetNeedAppearances(bool needAppearances)
        {
            _needAppearances = needAppearances;
            _dict.Remove("NeedAppearances");
            if (needAppearances)
            {
                _dict.Add("NeedAppearances", new HpdfBoolean(true));
            }
        }

        /// <summary>
        /// Gets whether the form needs appearances to be generated.
        /// </summary>
        public bool NeedAppearances => _needAppearances;

        /// <summary>
        /// Adds a field to the form.
        /// </summary>
        /// <param name="field">The field to add.</param>
        public void AddField(HpdfField field)
        {
            if (field == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Field cannot be null");

            _fields.Add(field);

            // Update the Fields array in the dictionary
            UpdateFieldsArray();
        }

        /// <summary>
        /// Sets the calculation order for fields.
        /// This determines the order in which field calculations are performed.
        /// </summary>
        /// <param name="fields">The fields in calculation order.</param>
        public void SetCalculationOrder(params HpdfField[] fields)
        {
            _dict.Remove("CO");
            if (fields != null && fields.Length > 0)
            {
                var coArray = new HpdfArray();
                foreach (var field in fields)
                {
                    if (field != null)
                    {
                        coArray.Add(field.Dict);
                    }
                }
                _dict.Add("CO", coArray);
            }
        }

        /// <summary>
        /// Updates the Fields array in the AcroForm dictionary.
        /// </summary>
        private void UpdateFieldsArray()
        {
            if (_fields.Count == 0)
            {
                _dict.Remove("Fields");
                return;
            }

            var fieldsArray = new HpdfArray();
            foreach (var field in _fields)
            {
                fieldsArray.Add(field.Dict);
            }

            // Remove existing Fields entry before adding new one
            _dict.Remove("Fields");
            _dict.Add("Fields", fieldsArray);
        }

        /// <summary>
        /// Prepares the AcroForm for writing to the PDF.
        /// This method should be called before saving the document.
        /// </summary>
        internal void PrepareForWriting()
        {
            // Ensure Fields array is up to date
            UpdateFieldsArray();
        }
    }
}
