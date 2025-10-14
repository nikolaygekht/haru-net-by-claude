using System.IO;
using FluentAssertions;
using Haru.Doc;
using Haru.Forms;
using Haru.Types;
using Xunit;

namespace Haru.Test
{
    public class HpdfAcroFormTests
    {
        [Fact]
        public void GetOrCreateAcroForm_CreatesAcroForm_WhenNotExists()
        {
            var doc = new HpdfDocument();

            var form = doc.GetOrCreateAcroForm();

            form.Should().NotBeNull();
            form.Fields.Should().BeEmpty();
            form.NeedAppearances.Should().BeTrue();
        }

        [Fact]
        public void GetOrCreateAcroForm_ReturnsSameInstance_WhenCalledMultipleTimes()
        {
            var doc = new HpdfDocument();

            var form1 = doc.GetOrCreateAcroForm();
            var form2 = doc.GetOrCreateAcroForm();

            form1.Should().BeSameAs(form2);
        }

        [Fact]
        public void AcroForm_SetsPdfVersion15_WhenSaved()
        {
            var doc = new HpdfDocument();
            doc.Version.Should().Be(HpdfVersion.Version12); // Default

            var form = doc.GetOrCreateAcroForm();
            var field = new HpdfTextField(doc.Xref, "TestField");
            form.AddField(field);

            using (var ms = new MemoryStream())
            {
                doc.Save(ms);
                doc.Version.Should().Be(HpdfVersion.Version15);
            }
        }

        [Fact]
        public void TextField_CanBeCreatedAndAddedToForm()
        {
            var doc = new HpdfDocument();
            var form = doc.GetOrCreateAcroForm();

            var textField = new HpdfTextField(doc.Xref, "Name");
            textField.SetValue("John Doe");
            textField.SetMaxLength(50);
            form.AddField(textField);

            form.Fields.Should().HaveCount(1);
            form.Fields[0].Should().BeSameAs(textField);
        }

        [Fact]
        public void TextField_SupportsMultilineFlag()
        {
            var doc = new HpdfDocument();
            var textField = new HpdfTextField(doc.Xref, "Comments");

            textField.SetMultiline(true);

            textField.Flags.Should().HaveFlag(HpdfFieldFlags.Multiline);
        }

        [Fact]
        public void TextField_SupportsPasswordFlag()
        {
            var doc = new HpdfDocument();
            var textField = new HpdfTextField(doc.Xref, "Password");

            textField.SetPassword(true);

            textField.Flags.Should().HaveFlag(HpdfFieldFlags.Password);
        }

        [Fact]
        public void Checkbox_CanBeCreatedAndSetChecked()
        {
            var doc = new HpdfDocument();
            var checkbox = new HpdfCheckbox(doc.Xref, "AgreeToTerms");

            checkbox.SetChecked(true);

            checkbox.Dict["V"].Should().NotBeNull();
        }

        [Fact]
        public void RadioButton_CanHaveMultipleOptions()
        {
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            var radioButton = new HpdfRadioButton(doc.Xref, "Gender");

            radioButton.CreateOption("Male", new HpdfRect(50, 100, 70, 120));
            radioButton.CreateOption("Female", new HpdfRect(50, 130, 70, 150));

            radioButton.Widgets.Should().HaveCount(2);
            radioButton.Flags.Should().HaveFlag(HpdfFieldFlags.Radio);
        }

        [Fact]
        public void RadioButton_CanSetSelectedValue()
        {
            var doc = new HpdfDocument();
            var radioButton = new HpdfRadioButton(doc.Xref, "Color");

            var widget1 = radioButton.CreateOption("Red", new HpdfRect(0, 0, 20, 20));
            var widget2 = radioButton.CreateOption("Blue", new HpdfRect(0, 0, 20, 20));

            radioButton.SetSelected("Blue");

            radioButton.Dict["V"].Should().NotBeNull();
        }

        [Fact]
        public void Choice_CanBeCreatedAsComboBox()
        {
            var doc = new HpdfDocument();
            var choice = new HpdfChoice(doc.Xref, "Country", isComboBox: true);

            choice.Flags.Should().HaveFlag(HpdfFieldFlags.Combo);
        }

        [Fact]
        public void Choice_CanBeCreatedAsListBox()
        {
            var doc = new HpdfDocument();
            var choice = new HpdfChoice(doc.Xref, "Colors", isComboBox: false);

            choice.Flags.Should().NotHaveFlag(HpdfFieldFlags.Combo);
        }

        [Fact]
        public void Choice_CanAddMultipleOptions()
        {
            var doc = new HpdfDocument();
            var choice = new HpdfChoice(doc.Xref, "Country", isComboBox: true);

            choice.AddOptions("USA", "Canada", "Mexico");

            choice.Options.Should().HaveCount(3);
            choice.Options.Should().Contain("USA");
            choice.Options.Should().Contain("Canada");
            choice.Options.Should().Contain("Mexico");
        }

        [Fact]
        public void Choice_CanSetSelectedValue()
        {
            var doc = new HpdfDocument();
            var choice = new HpdfChoice(doc.Xref, "Country", isComboBox: true);
            choice.AddOptions("USA", "Canada", "Mexico");

            choice.SetSelectedValues("Canada");

            choice.Dict["V"].Should().NotBeNull();
        }

        [Fact]
        public void Choice_CanSupportMultiSelect()
        {
            var doc = new HpdfDocument();
            var choice = new HpdfChoice(doc.Xref, "Colors", isComboBox: false);

            choice.SetMultiSelect(true);

            choice.Flags.Should().HaveFlag(HpdfFieldFlags.MultiSelect);
        }

        [Fact]
        public void Signature_CreatesUnsignedField()
        {
            var doc = new HpdfDocument();
            var signature = new HpdfSignature(doc.Xref, "SignatureField");

            // An unsigned signature field should NOT have a "V" (value) entry
            signature.Dict.ContainsKey("V").Should().BeFalse();

            // It should have the correct field type
            signature.FieldType.Should().Be(HpdfFieldType.Sig);
        }

        [Fact]
        public void Field_CanSetReadOnlyFlag()
        {
            var doc = new HpdfDocument();
            var textField = new HpdfTextField(doc.Xref, "ReadOnlyField");

            textField.SetReadOnly(true);

            textField.Flags.Should().HaveFlag(HpdfFieldFlags.ReadOnly);
        }

        [Fact]
        public void Field_CanSetRequiredFlag()
        {
            var doc = new HpdfDocument();
            var textField = new HpdfTextField(doc.Xref, "RequiredField");

            textField.SetRequired(true);

            textField.Flags.Should().HaveFlag(HpdfFieldFlags.Required);
        }

        [Fact]
        public void Widget_CanBeAddedToPage()
        {
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            var textField = new HpdfTextField(doc.Xref, "PageField");

            var widget = textField.CreateWidget(new HpdfRect(50, 50, 200, 70));
            page.AddWidgetAnnotation(widget);

            textField.Widgets.Should().HaveCount(1);
            page.Dict["Annots"].Should().NotBeNull();
        }

        [Fact]
        public void AcroForm_CanSetDefaultAppearance()
        {
            var doc = new HpdfDocument();
            var form = doc.GetOrCreateAcroForm();

            form.SetDefaultAppearance("/Helv 14 Tf 0 0 1 rg");

            form.DefaultAppearance.Should().Be("/Helv 14 Tf 0 0 1 rg");
        }

        [Fact]
        public void AcroForm_CanSetCalculationOrder()
        {
            var doc = new HpdfDocument();
            var form = doc.GetOrCreateAcroForm();
            var field1 = new HpdfTextField(doc.Xref, "Field1");
            var field2 = new HpdfTextField(doc.Xref, "Field2");

            form.AddField(field1);
            form.AddField(field2);
            form.SetCalculationOrder(field1, field2);

            form.Dict["CO"].Should().NotBeNull();
        }

        [Fact]
        public void CompleteForm_CanBeSavedToPdf()
        {
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            var form = doc.GetOrCreateAcroForm();

            // Add text field
            var nameField = new HpdfTextField(doc.Xref, "Name");
            nameField.SetValue("John Doe");
            var nameWidget = nameField.CreateWidget(new HpdfRect(100, 700, 300, 720));
            page.AddWidgetAnnotation(nameWidget);
            form.AddField(nameField);

            // Add checkbox
            var checkbox = new HpdfCheckbox(doc.Xref, "AgreeToTerms");
            checkbox.SetChecked(true);
            var checkWidget = checkbox.CreateWidget(new HpdfRect(100, 650, 120, 670));
            page.AddWidgetAnnotation(checkWidget);
            form.AddField(checkbox);

            // Save to memory
            byte[] pdfData;
            using (var ms = new MemoryStream())
            {
                doc.Save(ms);
                pdfData = ms.ToArray();
            }

            pdfData.Should().NotBeEmpty();
            pdfData.Length.Should().BeGreaterThan(100);
        }
    }
}
