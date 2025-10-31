/*
 * << Haru Free PDF Library >> -- AnnotationsDemo.cs
 *
 * Demonstrates text annotations (sticky notes) with different icons and colors
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using System;
using Haru.Annotations;
using Haru.Doc;
using Haru.Font;
using Haru.Types;

namespace BasicDemos
{
    public static class AnnotationsDemo
    {
        public static void Run()
        {
            try
            {
                var pdf = new HpdfDocument();
                pdf.SetCompressionMode(HpdfCompressionMode.All);

                var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
                var boldFont = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaBold, "F2");

                CreateIconTypesPage(pdf, font, boldFont);
                CreateDocumentReviewPage(pdf, font, boldFont);

                pdf.SaveToFile("pdfs/AnnotationsDemo.pdf");
                Console.WriteLine("AnnotationsDemo completed successfully!");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in AnnotationsDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }

        private static void CreateIconTypesPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            DrawLabel(page, boldFont, 50, height - 50, "Text Annotations - Icon Types", 24);
            DrawLabel(page, font, 50, height - 75, "Sticky notes with different visual indicators", 12);

            float y = height - 120;
            float x = 70;
            float spacing = 70;

            DrawLabel(page, font, 50, y, "All Seven Icon Types:");
            DrawLabel(page, font, 50, y - 15, "(Click the icons to view annotation content)", 10, 0.4f, 0.4f, 0.4f);

            y -= 50;

            var iconTypes = new[]
            {
                HpdfAnnotationIcon.Comment,
                HpdfAnnotationIcon.Key,
                HpdfAnnotationIcon.Note,
                HpdfAnnotationIcon.Help,
                HpdfAnnotationIcon.NewParagraph,
                HpdfAnnotationIcon.Paragraph,
                HpdfAnnotationIcon.Insert
            };

            string[] iconNames = { "Comment", "Key", "Note", "Help", "NewParagraph", "Paragraph", "Insert" };
            string[] descriptions =
            {
                "General comment or feedback",
                "Important or security-related note",
                "Standard note (default icon)",
                "Help or question",
                "Start new paragraph here",
                "Paragraph marker",
                "Insert text here"
            };

            for (int i = 0; i < iconTypes.Length; i++)
            {
                var rect = new HpdfRect(x + i * spacing, y - 20, x + i * spacing + 20, y);
                var annotation = page.CreateTextAnnotation(
                    rect,
                    $"This is a '{iconNames[i]}' annotation. {descriptions[i]}.",
                    iconTypes[i]
                );
                annotation.SetRgbColor(1.0f, 0.8f, 0.0f);

                DrawSmallLabel(page, font, x + i * spacing + 10, y - 30, iconNames[i]);
            }

            y -= 90;

            DrawLabel(page, font, 50, y, "Different Colors:");
            DrawLabel(page, font, 50, y - 15, "(Annotations can use any RGB color)", 10, 0.4f, 0.4f, 0.4f);

            y -= 50;

            float[,] colors =
            {
                { 1.0f, 0.0f, 0.0f },
                { 1.0f, 0.5f, 0.0f },
                { 1.0f, 1.0f, 0.0f },
                { 0.0f, 0.8f, 0.0f },
                { 0.0f, 0.5f, 1.0f },
                { 0.5f, 0.0f, 1.0f }
            };

            string[] colorNames = { "Red", "Orange", "Yellow", "Green", "Blue", "Purple" };

            for (int i = 0; i < colors.GetLength(0); i++)
            {
                var rect = new HpdfRect(x + i * spacing, y - 20, x + i * spacing + 20, y);
                var annotation = page.CreateTextAnnotation(
                    rect,
                    $"This is a {colorNames[i].ToLower()} annotation. Colors help categorize or prioritize notes.",
                    HpdfAnnotationIcon.Note
                );
                annotation.SetRgbColor(colors[i, 0], colors[i, 1], colors[i, 2]);

                DrawSmallLabel(page, font, x + i * spacing + 10, y - 30, colorNames[i]);
            }

            y -= 100;

            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(50, y - 100, width - 100, 90);
            page.Stroke();

            DrawLabel(page, font, 60, y - 20, "Text Annotation Features:", 11);
            DrawLabel(page, font, 60, y - 38, "• Seven icon types for different purposes (Comment, Key, Note, Help, NewParagraph, Paragraph, Insert)", 10);
            DrawLabel(page, font, 60, y - 53, "• Customizable colors for categorization", 10);
            DrawLabel(page, font, 60, y - 68, "• Perfect for document review, collaboration, and feedback workflows", 10);
        }

        private static void CreateDocumentReviewPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            DrawLabel(page, boldFont, 50, height - 50, "Practical Example - Document Review", 24);
            DrawLabel(page, font, 50, height - 75, "Collaborative editing and feedback workflow", 12);

            float y = height - 120;

            DrawLabel(page, boldFont, 50, y, "Project Proposal Draft", 16);
            y -= 30;

            DrawLabel(page, font, 70, y, "Executive Summary", 12);
            y -= 20;
            DrawLabel(page, font, 70, y, "This proposal outlines our plan to develop a new customer portal that will", 11);
            y -= 15;
            DrawLabel(page, font, 70, y, "streamline user interactions and improve overall satisfaction. The project is", 11);
            y -= 15;
            DrawLabel(page, font, 70, y, "estimated to take six months and require a budget of $500,000.", 11);

            var commentRect = new HpdfRect(520, y + 5, 540, y + 25);
            var comment = page.CreateTextAnnotation(
                commentRect,
                "Great summary! Very clear and concise. This gives a good overview of the project scope.",
                HpdfAnnotationIcon.Comment
            );
            comment.SetRgbColor(0.0f, 0.7f, 0.0f);

            y -= 35;
            DrawLabel(page, font, 70, y, "Technical Requirements", 12);
            y -= 20;
            DrawLabel(page, font, 70, y, "The portal will be built using modern web technologies including React,", 11);
            y -= 15;
            DrawLabel(page, font, 70, y, "Node.js, and PostgreSQL. We will implement OAuth2 for secure authentication", 11);
            y -= 15;
            DrawLabel(page, font, 70, y, "and follow WCAG 2.1 accessibility guidelines.", 11);

            var keyRect = new HpdfRect(520, y + 5, 540, y + 25);
            var keyAnnot = page.CreateTextAnnotation(
                keyRect,
                "IMPORTANT: We should also consider adding two-factor authentication for enhanced security.",
                HpdfAnnotationIcon.Key
            );
            keyAnnot.SetRgbColor(1.0f, 0.0f, 0.0f);

            y -= 35;
            DrawLabel(page, font, 70, y, "Timeline and Milestones", 12);
            y -= 20;
            DrawLabel(page, font, 70, y, "Phase 1: Requirements gathering (4 weeks)", 11);

            var insertRect1 = new HpdfRect(340, y + 5, 360, y + 25);
            var insertAnnot1 = page.CreateTextAnnotation(
                insertRect1,
                "Add: Include stakeholder interviews in this phase.",
                HpdfAnnotationIcon.Insert
            );
            insertAnnot1.SetRgbColor(1.0f, 0.5f, 0.0f);

            y -= 15;
            DrawLabel(page, font, 70, y, "Phase 2: Design and prototyping (6 weeks)", 11);
            y -= 15;
            DrawLabel(page, font, 70, y, "Phase 3: Development (12 weeks)", 11);

            var helpRect = new HpdfRect(280, y + 5, 300, y + 25);
            var helpAnnot = page.CreateTextAnnotation(
                helpRect,
                "Question: Should we break this into smaller sprints? 12 weeks seems like a long phase without milestones.",
                HpdfAnnotationIcon.Help
            );
            helpAnnot.SetRgbColor(0.0f, 0.5f, 1.0f);

            y -= 15;
            DrawLabel(page, font, 70, y, "Phase 4: Testing and QA (4 weeks)", 11);
            y -= 15;
            DrawLabel(page, font, 70, y, "Phase 5: Deployment and training (2 weeks)", 11);

            y -= 35;
            DrawLabel(page, font, 70, y, "Budget Breakdown", 12);
            y -= 20;
            DrawLabel(page, font, 70, y, "Development: $300,000 | Design: $100,000 | Infrastructure: $100,000", 11);

            var noteRect = new HpdfRect(520, y + 5, 540, y + 25);
            var noteAnnot = page.CreateTextAnnotation(
                noteRect,
                "Note: Budget looks reasonable. Make sure to include a contingency fund of 10-15% for unexpected costs.",
                HpdfAnnotationIcon.Note
            );
            noteAnnot.SetRgbColor(1.0f, 1.0f, 0.0f);

            y -= 35;
            DrawLabel(page, font, 70, y, "The portal will integrate with existing systems including our CRM, billing", 11);
            y -= 15;
            DrawLabel(page, font, 70, y, "platform, and support ticketing system. We expect this to significantly reduce", 11);
            y -= 15;
            DrawLabel(page, font, 70, y, "manual data entry and improve data accuracy.", 11);

            var newParaRect = new HpdfRect(50, y + 5, 70, y + 25);
            var newParaAnnot = page.CreateTextAnnotation(
                newParaRect,
                "Consider starting a new section here about 'Integration Strategy' - this deserves its own detailed section.",
                HpdfAnnotationIcon.NewParagraph
            );
            newParaAnnot.SetRgbColor(0.5f, 0.0f, 1.0f);

            y -= 50;

            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(50, y - 80, width - 100, 70);
            page.Stroke();

            DrawLabel(page, font, 60, y - 20, "Review Workflow Benefits:", 11);
            DrawLabel(page, font, 60, y - 35, "• Annotations preserve original document while adding feedback", 10);
            DrawLabel(page, font, 60, y - 50, "• Different icons help categorize feedback (suggestions, questions, issues, approvals)", 10);
            DrawLabel(page, font, 60, y - 65, "• Colors enable priority levels (red = critical, yellow = review, green = approved)", 10);
        }

        private static void DrawLabel(HpdfPage page, HpdfFont font, float x, float y, string text,
            float size = 11, float r = 0, float g = 0, float b = 0)
        {
            page.SetFontAndSize(font, size);
            page.SetRgbFill(r, g, b);
            page.BeginText();
            page.MoveTextPos(x, y);
            page.ShowText(text);
            page.EndText();
        }

        private static void DrawSmallLabel(HpdfPage page, HpdfFont font, float x, float y, string text)
        {
            page.SetFontAndSize(font, 8);
            page.SetRgbFill(0, 0, 0);
            page.BeginText();
            float textWidth = font.MeasureText(text, 8);
            page.MoveTextPos(x - textWidth / 2, y);
            page.ShowText(text);
            page.EndText();
        }
    }
}
