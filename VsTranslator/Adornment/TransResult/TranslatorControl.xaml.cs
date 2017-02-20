﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Text;
using VsTranslator.Adornment.TransResult;
using VsTranslator.Core.Translator.Enums;
using VsTranslator.Settings;

namespace VsTranslator.Adornment.TransResult
{
    /// <summary>
    /// Interaction logic for TranslatorControl.xaml
    /// </summary>
    public partial class TranslatorControl : UserControl
    {

        private TranslatorControl()
        {
            InitializeComponent();

        }

        private static SnapshotSpan _selectedSpans;


        public TranslatorControl(SnapshotSpan selectedSpans, TranslationRequest transRequest)
        {
            InitializeComponent();
            _selectedSpans = selectedSpans;
            transRequest.OnTranslationComplete += TransRequest_OnTranslationComplete;

            transRequest.OnAllTranslationComplete += TransRequest_OnAllTranslationComplete;
        }

        private void TransRequest_OnAllTranslationComplete()
        {
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    lbltitle.Text = "Translation successed..";
            //}));
        }

        /// <summary>
        /// Append translate result text to Translate Control 
        /// </summary>
        /// <param name="translationResult"></param>
        private void TransRequest_OnTranslationComplete(TranslateResult translationResult)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lbltitle.Text = $"[{translationResult.Identity}]({translationResult.SourceLanguage} - {translationResult.TargetLanguage})";
                if (translationResult.TranslationResultTypes == TranslationResultTypes.Successed)
                {
                    AppendTargetText(translationResult.Identity, translationResult.SourceLanguage, translationResult.TargetLanguage, translationResult.TargetText);
                }
                else
                {
                    lbltitle.Foreground = new SolidColorBrush(Colors.Red);
                    lbltitle.Text = translationResult.FailedReason;
                }
            }));
        }

        private void AppendTargetText(string identity, string sourceLanguage, string targetLanguage, string targetText)
        {
            var wrapPanel = new WrapPanel();

            //System.Windows.Controls.Image image = new System.Windows.Controls.Image
            //{
            //    Source = new BitmapImage(new Uri("pack://application:,,,/VsTranslator;component/Resources/google_16.ico")),
            //    Width = 14,
            //    Height = 14
            //};
            //wrapPanel.Children.Add(image);


            var label = new TextBlock()
            {
                Text = targetText,
                TextWrapping = TextWrapping.Wrap,
                ToolTip = "click to replace selcted text with this translation",
                FontWeight = FontWeights.Bold,
                Padding = new Thickness(3, 1, 3, 1),
                MinWidth = 180
            };

            label.MouseDown += Label_MouseDown;

            wrapPanel.SetResourceReference(StyleProperty, "MouseOver");
            wrapPanel.Children.Add(label);

            transResult.Children.Add(wrapPanel);
        }

        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock label = sender as TextBlock;
            if (label == null)
            {
                return;
            }
            ReplaceSelectedText(label.Text);
            e.Handled = true;
            RemoveEvent?.Invoke();
        }

        /// <summary>
        /// Replace selected text in vs code editor when click the translate result text
        /// </summary>
        /// <param name="targetText"></param>
        public static void ReplaceSelectedText(string targetText)
        {
            var span = _selectedSpans.Snapshot.CreateTrackingSpan(_selectedSpans, SpanTrackingMode.EdgeExclusive);
            ITextBuffer buffer = span.TextBuffer;
            var sp = span.GetSpan(buffer.CurrentSnapshot);
            buffer.Replace(sp, targetText);
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            //Global.Dte.ExecuteCommand("Tools.Options", GuidList.TranslateOptions);
            OptionsSettings.ShowOptions();
        }

        public Action RemoveEvent;

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            RemoveEvent?.Invoke();
        }
    }
}
