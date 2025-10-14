/*
 * << Haru Free PDF Library >> -- AcroFormsDemo.cs
 *
 * Demonstrates PDF AcroForms (interactive form fields)
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using System;
using Haru.Doc;
using Haru.Font;
using Haru.Forms;
using Haru.Graphics;
using Haru.Types;

namespace BasicDemos
{
    public static class AcroFormsDemo
    {
        public static void Run()
        {
            try
            {
                Console.WriteLine("Creating AcroForms Demo...");

                var pdf = new HpdfDocument();

                // Get the AcroForm container
                var form = pdf.GetOrCreateAcroForm();

                // Create page
                var page = pdf.AddPage(HpdfPageSize.A4, Haru.Doc.HpdfPageDirection.Portrait);
                float pageWidth = page.Width;
                float pageHeight = page.Height;

                // Create font for labels
                var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
                var fontBold = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaBold, "F2");
                page.SetFontAndSize(fontBold, 16);

                // Title
                page.BeginText();
                page.MoveTextPos(50, pageHeight - 50);
                page.ShowText("Interactive Form Demo");
                page.EndText();

                // Helper to draw field labels
                void DrawLabel(float x, float y, string text)
                {
                    page.SetFontAndSize(font, 11);
                    page.BeginText();
                    page.MoveTextPos(x, y);
                    page.ShowText(text);
                    page.EndText();
                }

                float yPos = pageHeight - 100;
                float xLabel = 50;
                float xField = 200;
                float fieldWidth = 200;
                float fieldHeight = 20;

                // ===== TEXT FIELDS =====
                page.SetFontAndSize(fontBold, 12);
                page.BeginText();
                page.MoveTextPos(50, yPos);
                page.ShowText("1. Text Fields");
                page.EndText();
                yPos -= 30;

                // Single-line text field
                DrawLabel(xLabel, yPos - 5, "Name:");
                var nameField = new HpdfTextField(pdf.Xref, "Name");
                nameField.SetValue("John Doe");
                nameField.SetRequired(true);
                var nameWidget = nameField.CreateWidget(new HpdfRect(xField, yPos - fieldHeight, xField + fieldWidth, yPos));
                page.AddWidgetAnnotation(nameWidget);
                form.AddField(nameField);
                yPos -= (fieldHeight + 10);

                // Email field
                DrawLabel(xLabel, yPos - 5, "Email:");
                var emailField = new HpdfTextField(pdf.Xref, "Email");
                emailField.SetValue("john@example.com");
                var emailWidget = emailField.CreateWidget(new HpdfRect(xField, yPos - fieldHeight, xField + fieldWidth, yPos));
                page.AddWidgetAnnotation(emailWidget);
                form.AddField(emailField);
                yPos -= (fieldHeight + 10);

                // Password field
                DrawLabel(xLabel, yPos - 5, "Password:");
                var passwordField = new HpdfTextField(pdf.Xref, "Password");
                passwordField.SetPassword(true);
                var passwordWidget = passwordField.CreateWidget(new HpdfRect(xField, yPos - fieldHeight, xField + fieldWidth, yPos));
                page.AddWidgetAnnotation(passwordWidget);
                form.AddField(passwordField);
                yPos -= (fieldHeight + 10);

                // Multi-line text field
                float commentsHeight = 80;
                DrawLabel(xLabel, yPos - 5, "Comments:");
                var commentsField = new HpdfTextField(pdf.Xref, "Comments");
                commentsField.SetMultiline(true);
                commentsField.SetValue("Enter your comments here...");
                var commentsWidget = commentsField.CreateWidget(new HpdfRect(xField, yPos - commentsHeight, xField + fieldWidth, yPos));
                page.AddWidgetAnnotation(commentsWidget);
                form.AddField(commentsField);
                yPos -= (commentsHeight + 10);

                // ===== CHECKBOXES =====
                page.SetFontAndSize(fontBold, 12);
                page.BeginText();
                page.MoveTextPos(50, yPos);
                page.ShowText("2. Checkboxes");
                page.EndText();
                yPos -= 30;

                // Checkbox 1
                float checkboxSize = 15;
                var agreeCheckbox = new HpdfCheckbox(pdf.Xref, "AgreeToTerms");
                agreeCheckbox.SetChecked(true);
                var agreeWidget = agreeCheckbox.CreateWidget(new HpdfRect(xLabel, yPos - checkboxSize, xLabel + checkboxSize, yPos));
                page.AddWidgetAnnotation(agreeWidget);
                form.AddField(agreeCheckbox);
                DrawLabel(xLabel + 25, yPos - 13, "I agree to the terms and conditions");
                yPos -= (checkboxSize + 10);

                // Checkbox 2
                var newsletterCheckbox = new HpdfCheckbox(pdf.Xref, "Newsletter");
                newsletterCheckbox.SetChecked(false);
                var newsletterWidget = newsletterCheckbox.CreateWidget(new HpdfRect(xLabel, yPos - checkboxSize, xLabel + checkboxSize, yPos));
                page.AddWidgetAnnotation(newsletterWidget);
                form.AddField(newsletterCheckbox);
                DrawLabel(xLabel + 25, yPos - 13, "Subscribe to newsletter");
                yPos -= (checkboxSize + 10);

                // ===== RADIO BUTTONS =====
                page.SetFontAndSize(fontBold, 12);
                page.BeginText();
                page.MoveTextPos(50, yPos);
                page.ShowText("3. Radio Buttons");
                page.EndText();
                yPos -= 30;

                float radioSize = 15;
                DrawLabel(xLabel, yPos - 13, "Gender:");

                var genderRadio = new HpdfRadioButton(pdf.Xref, "Gender");
                var maleWidget = genderRadio.CreateOption("Male", new HpdfRect(xField, yPos - radioSize, xField + radioSize, yPos));
                page.AddWidgetAnnotation(maleWidget);
                DrawLabel(xField + 20, yPos - 13, "Male");

                var femaleWidget = genderRadio.CreateOption("Female", new HpdfRect(xField + 80, yPos - radioSize, xField + 95, yPos));
                page.AddWidgetAnnotation(femaleWidget);
                DrawLabel(xField + 100, yPos - 13, "Female");

                genderRadio.SetSelected("Male");
                form.AddField(genderRadio);
                yPos -= (radioSize + 10);

                // ===== CHOICE FIELDS =====
                page.SetFontAndSize(fontBold, 12);
                page.BeginText();
                page.MoveTextPos(50, yPos);
                page.ShowText("4. Choice Fields");
                page.EndText();
                yPos -= 30;

                // Combo box (dropdown)
                DrawLabel(xLabel, yPos - 5, "Country:");
                var countryChoice = new HpdfChoice(pdf.Xref, "Country", isComboBox: true);
                countryChoice.AddOptions("USA", "Canada", "Mexico", "UK", "Germany", "France", "Japan");
                countryChoice.SetSelectedValues("USA");
                var countryWidget = countryChoice.CreateWidget(new HpdfRect(xField, yPos - fieldHeight, xField + fieldWidth, yPos));
                page.AddWidgetAnnotation(countryWidget);
                form.AddField(countryChoice);
                yPos -= (fieldHeight + 10);

                // List box
                float skillsHeight = 60;
                DrawLabel(xLabel, yPos - 5, "Skills:");
                var skillsChoice = new HpdfChoice(pdf.Xref, "Skills", isComboBox: false);
                skillsChoice.AddOptions("C#", "Python", "JavaScript", "Java", "C++");
                skillsChoice.SetMultiSelect(true);
                var skillsWidget = skillsChoice.CreateWidget(new HpdfRect(xField, yPos - skillsHeight, xField + fieldWidth, yPos));
                page.AddWidgetAnnotation(skillsWidget);
                form.AddField(skillsChoice);
                yPos -= (skillsHeight + 10);

                // ===== SIGNATURE FIELD =====
                page.SetFontAndSize(fontBold, 12);
                page.BeginText();
                page.MoveTextPos(50, yPos);
                page.ShowText("5. Signature Field");
                page.EndText();
                yPos -= 30;

                float signatureHeight = 40;
                DrawLabel(xLabel, yPos - 5, "Signature:");
                var signatureField = new HpdfSignature(pdf.Xref, "Signature");
                // Note: This creates an unsigned signature field that can be signed by the user
                var signatureWidget = signatureField.CreateWidget(new HpdfRect(xField, yPos - signatureHeight, xField + fieldWidth, yPos));
                page.AddWidgetAnnotation(signatureWidget);
                form.AddField(signatureField);

                // Draw decorative border around signature field
                page.SetRgbStroke(0.7f, 0.7f, 0.7f);
                page.SetLineWidth(1);
                page.Rectangle(xField, yPos - signatureHeight, fieldWidth, signatureHeight);
                page.Stroke();

                // Save the document
                pdf.SaveToFile("pdfs/AcroFormsDemo.pdf");

                Console.WriteLine("AcroForms Demo created successfully!");
                Console.WriteLine("PDF saved to: pdfs/AcroFormsDemo.pdf");
                Console.WriteLine("PDF Version: " + pdf.Version);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in AcroFormsDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }
    }
}
