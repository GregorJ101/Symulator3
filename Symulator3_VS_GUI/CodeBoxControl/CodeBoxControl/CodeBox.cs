using System;                     // Console, Math, ArgumentOutOfRangeException
//using System.ComponentModel;
using System.Collections.Generic; // List
using System.Globalization;       // CultureInfo
using System.Threading;           // Sleep
using System.Windows;             // DependencyProperty, FrameworkPropertyMetadataOptions, Rect, FlowDirection, Point, DependencyObject
using System.Windows.Controls;    // TextBox. TextChangedEventArgs, ScrollChangedEventArgs
using System.Windows.Media;       // Brush, SolidColorBrush

// Based on the code from https://www.codeproject.com/Articles/33939/CodeBox
// Thank you Ken Johnson for pointing me in the right direction
namespace CodeBoxControl
{
    /// <summary>
    ///  A control to view or edit styled text
    /// </summary>
    public partial class CodeBox : TextBox
    {
        #region Data members
        public enum SyntaxColoringMode
        {
            NoColoring,
            Dasm,
            Trace,
        }

        bool                               m_bScrollingEventEnabled;
        volatile bool                      m_bPauseOutput = false;
        volatile bool                      m_bEnableHighlightLine = false;
        volatile bool                      m_bSuspendRendering = false;
        volatile int                       m_iCurrentLineIdx = -1;
        object                             m_objLock = null;
        private SyntaxColoringMode         m_eSyntaxColoringMode = SyntaxColoringMode.NoColoring;
        private SortedDictionary<int, int> m_sdLineAddresses = new SortedDictionary<int, int> ();
        private List<int>                  m_liGrayedCode = new List<int> ();

        SolidColorBrush m_brCoral     = new SolidColorBrush (Colors.Coral);
        SolidColorBrush m_brRoyalBlue = new SolidColorBrush (Colors.RoyalBlue);
        SolidColorBrush m_brBlack     = new SolidColorBrush (Colors.Black);
        SolidColorBrush m_brMedSeaGrn = new SolidColorBrush (Colors.MediumSeaGreen);
        SolidColorBrush m_brFirebrick = new SolidColorBrush (Colors.Firebrick);
        SolidColorBrush m_brTeal      = new SolidColorBrush (Colors.Teal); //Color.FromRgb (0x2B, 0x91, 0xAF)); // Teal
        SolidColorBrush m_brFuchsia   = new SolidColorBrush (Colors.Fuchsia); // m_brLtPurple (Color.FromRgb (0xD4, 0x00, 0xFA)); // #D400FA  Light Purple
        SolidColorBrush m_brPurple    = new SolidColorBrush (Colors.Purple); // (Color.FromRgb (0x8B, 0x00, 0xA4)); // #8B00A4  Purple
        SolidColorBrush m_brDkGreen   = new SolidColorBrush (Colors.DarkGreen); // (Color.FromRgb (0x00, 0x80, 0x00)); // #008000  Dark green
        SolidColorBrush m_brYellow    = new SolidColorBrush (Colors.Yellow); // (Color.FromRgb (0xEC, 0xFE, 0x55)); // 0, 13
        SolidColorBrush m_brBrkPt     = new SolidColorBrush (Colors.Red); // (Color.FromRgb (0xFD, 0xCF, 0xCF)); // 13, 7
        SolidColorBrush m_brDisBrkPt  = new SolidColorBrush (Colors.LightPink); // (Color.FromRgb (0xF8, 0xE2, 0xE2)); // 13, 7
        SolidColorBrush m_brBkMrk     = new SolidColorBrush (Color.FromRgb (0x00, 0xEF, 0xFF)); // (Colors.LightSkyBlue); // (Color.FromRgb (0x00, 0xC6, 0xFF)); // 26, 11
        SolidColorBrush m_brLtGray    = new SolidColorBrush (Colors.LightGray);
        //SolidColorBrush m_brBkGround  = new SolidColorBrush (Color.FromRgb (0xCE, 0xE9, 0xC9)); // From Visual Studio settings
        #endregion

        public CodeBox ()
        {
            this.TextChanged += new TextChangedEventHandler (txtTest_TextChanged);
            this.Foreground = new SolidColorBrush (Colors.Transparent);
            this.Background = new SolidColorBrush (Colors.Transparent);
            InitializeComponent ();
        }

        public void SetLockObject (ref object objLock)
        {
            m_objLock = objLock;
        }

        //public void OnHomeKey ()
        //{
        //    ScrollToHome ();
        //}

        //public void OnEndKey ()
        //{
        //    ScrollToEnd ();
        //}

        //public void OnPageUpKey ()
        //{
        //    int iFirstVisibleLine = GetFirstVisibleLineIndex ();
        //    int iLastVisibleLine  = GetLastVisibleLineIndex ();
        //    int iLinesVisible     = iLastVisibleLine - iFirstVisibleLine + 1;

        //    if (iFirstVisibleLine > 0)
        //    {
        //        if (iFirstVisibleLine > iLinesVisible)
        //        {
        //            ScrollToLine (iFirstVisibleLine - iLinesVisible);
        //        }
        //        else
        //        {
        //            ScrollToLine (0);
        //        }
        //    }
        //}

        //public void OnPageDownKey ()
        //{
        //    int iFirstVisibleLine = GetFirstVisibleLineIndex ();
        //    int iLastVisibleLine  = GetLastVisibleLineIndex ();
        //    int iLinesVisible     = iLastVisibleLine - iFirstVisibleLine + 1;

        //    if (iLastVisibleLine < LineCount - 1)
        //    {
        //        if (iFirstVisibleLine < iLinesVisible)
        //        {
        //            ScrollToLine (iFirstVisibleLine + iLinesVisible - 1);
        //        }
        //        else
        //        {
        //            ScrollToLine (LineCount - 1);
        //        }
        //    }
        //}

        //public void OnUpArrowKey ()
        //{
        //    int iFirstVisibleLine = GetFirstVisibleLineIndex ();
        //    int iLastVisibleLine  = GetLastVisibleLineIndex ();
        //    int iLinesVisible     = iLastVisibleLine - iFirstVisibleLine + 1;
        //    LineUp ();
        //}

        //public void OnDownArrowKey ()
        //{
        //    int iFirstVisibleLine = GetFirstVisibleLineIndex ();
        //    int iLastVisibleLine  = GetLastVisibleLineIndex ();
        //    int iLinesVisible     = iLastVisibleLine - iFirstVisibleLine + 1;
        //    LineDown ();
        //}

        public void TogglePauseState ()
        {
            // TODO: Needs Render to be running in a separate thread
            //m_bPauseOutput = !m_bPauseOutput;
        }

        //public void SetGrayedCodeLines (ref SortedDictionary<int, int> rsdLineAddresses)
        //{
        //    m_liGrayedCode.Clear ();
        //    foreach (KeyValuePair<int, int> kvp in rsdLineAddresses)
        //    {
        //        if (kvp.Value != m_iCurrentLineIdx)
        //        {
        //            m_liGrayedCode.Add (kvp.Value);
        //        }
        //    }
        //}

        public void ClearGrayedCodeLines ()
        {
            m_liGrayedCode.Clear ();
            InvalidateVisual ();
        }

        //public void SetGrayedCodeLines (ref List<int> rliGrayedCode)
        //{
        //    m_liGrayedCode = rliGrayedCode;
        //}

        public void SetLineAddresses (ref SortedDictionary<int, int> rsdLineAddresses)
        {
            m_sdLineAddresses = rsdLineAddresses;

            //List<int> liGrayedCode = new List<int> ();
            m_liGrayedCode.Clear ();
            foreach (KeyValuePair<int, int> kvp in m_sdLineAddresses)
            {
                m_liGrayedCode.Add (kvp.Value);
            }
        }

        public void SetCurrentLineIdx (int iCurrentLineIdx)
        {
            m_iCurrentLineIdx      = iCurrentLineIdx;
            m_bEnableHighlightLine = true;

            if (m_liGrayedCode.Contains (iCurrentLineIdx))
            {
                m_liGrayedCode.Remove (iCurrentLineIdx);
            }

            if (m_iCurrentLineIdx > 0 &&
                m_iCurrentLineIdx < LineCount)
            {
                ScrollToLine (m_iCurrentLineIdx);
            }

            this.InvalidateVisual ();
        }

        public void EnableHighlightLine (bool bEnableHighlightLine)
        {
            if (m_bEnableHighlightLine != bEnableHighlightLine)
            {
                m_bEnableHighlightLine = bEnableHighlightLine;
                this.InvalidateVisual ();
            }
        }

        public void SuspendRendering (bool bSuspendRendering)
        {
            m_bSuspendRendering = bSuspendRendering;
            if (!m_bSuspendRendering)
            {
                InvalidateVisual ();
            }
        }

        public static DependencyProperty BaseForegroundProperty = DependencyProperty.Register ("BaseForeground", typeof (Brush), typeof (CodeBox),
                      new FrameworkPropertyMetadata (new SolidColorBrush (Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush BaseForeground
        {
            get { return (Brush)GetValue (BaseForegroundProperty); }
            set { SetValue (BaseForegroundProperty, value); }
        }

        public static DependencyProperty BaseBackgroundProperty = DependencyProperty.Register ("BaseBackground", typeof (Brush), typeof (CodeBox),
                      new FrameworkPropertyMetadata (new SolidColorBrush (Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush BaseBackground
        {
            get { return (Brush)GetValue (BaseBackgroundProperty); }
            set { SetValue (BaseBackgroundProperty, value); }
        }

        // This property can be set from XAML as an enum with all available enum values available for selection
        public static DependencyProperty SyntaxColoring = DependencyProperty.Register ("SyntaxColoring", typeof (SyntaxColoringMode), typeof (CodeBox),
                      new FrameworkPropertyMetadata (new SyntaxColoringMode (), FrameworkPropertyMetadataOptions.AffectsRender));

        public SyntaxColoringMode SyntaxColoringProperty
        {
            get { return m_eSyntaxColoringMode; }
            set { m_eSyntaxColoringMode = value; }
        }

        void txtTest_TextChanged (object sender, TextChangedEventArgs e)
        {
            this.InvalidateVisual ();
        }

        protected override void OnRender (System.Windows.Media.DrawingContext dcRendering)
        {
            if (m_bSuspendRendering)
            {
                return;
            }

            while (m_bPauseOutput)
            {
                Thread.Sleep (250);
            }

            if (this.Text.Length > 0)
            {
                int iStartVisibleLine = -1;
                int iEndVisibleLine   = -1;
                int iOffset           = -1;
                int iOffsetNext       = -1;
                string strLine        = "";

                EnsureScrolling ();
                FormattedText ftDisplayedLines = new FormattedText (
                    this.Text,
                    CultureInfo.GetCultureInfo ("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface (this.FontFamily.Source),
                    this.FontSize,
                    BaseForeground);  // Text that matches the textbox's
                double dLeftMargin  = 4.0 + this.BorderThickness.Left;
                double dTopMargin   = 2.0 + this.BorderThickness.Top;
                double dRightMargin = this.ActualWidth - 5.0; // Fix to long lines writing over right edge when horizontal scrollbar is preset

                ftDisplayedLines.MaxTextHeight = Math.Max (this.ActualHeight + this.VerticalOffset, 0); // Adjust for scrolling
                dcRendering.PushClip (new RectangleGeometry (new Rect (dLeftMargin, dTopMargin, dRightMargin, this.ActualHeight))); // Restrict text to textbox

                #region Text Coloring and Shading
                ftDisplayedLines.SetForegroundBrush (m_brBlack); // Default text color for any text not colored by offset / length

                if (m_eSyntaxColoringMode == SyntaxColoringMode.Dasm)
                #region DASM Output
                {
                    #region Comments
                    // Label:                0       Orange
                    // Instruction Address: 14       Red
                    // Mnemonic:            20       Blue
                    // Data Address:        43       Purple
                    // Annotation:          51       Date green
                    // Data1                26 / 25
                    // Data2                52
                    // Data3                62 / 4
                    // Data4                75
                    //           1         2         3         4         5         6         7         8         9         0         1
                    // 012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456
                    // Loop_02_0A0F  0A0F: SIO   F3 10 28                 5475 Keyboard  Set Error Indicator  Restore Data Key              
                    //               0A12: SNS   70 12 FF,1       0x0B0C  5475 Keyboard  2 sense bytes                                      
                    //               0A20: data  F1 6B F1 F2  6B F1 F3 40  <.k.. k..@>  <1,12 ,13 >

                    // Background for breakpoints (R:253 G:207 B:207)                             #FDCFCF
                    // Background for disabled breakpoints (R:248 G:226 B:226)                    #F8E2E2
                    // Background for active bookmark (R:0 G:198 B:255)                           #00C6FF
                    // Background for disabled bookmark (R:0 G:239 B:255)                         #00EFFF
                    // Background for current active line (R:236 G:254 B:85)                      #ECFE55

                    // Mnemonics: blue (R:0 G:0 B:255)                                                            #0000FF
                    // Operand addresses & command codes: black (R:0 G:0 B:0)                                     #000000
                    // Instruction addresses & command codes: dark red (R:128 G:0 B:0)                            #800000
                    // Annotation: dark green (R:0 G:128 B:0) (same as comments in Visual Studio)                 #008000
                    // Register labels: purple (R:139 G:0 B:164)                                                  #8B00A4
                    // Register values: light purple (R:212 G:0 B:250)                                            #D400FA
                    // Changed register values: red (R:255 G:0 B:0)                                               #FF0000
                    // Operand labels: purple (R:139 G:0 B:164)                                                   #8B00A4
                    // Operand memory: magenta (R:255 G:0 B:255)                                                  #FF00FF
                    // Changed memory: red (R:255 G:0 B:0)                                                        #FF0000
                    // Reset condition register flags: gray (R:128 G:128 B:128)                                   #808080
                    #endregion
                    iStartVisibleLine = GetFirstVisibleLineIndex ();
                    iEndVisibleLine   = GetLastVisibleLineIndex ();

                    for (int iIdx = iStartVisibleLine; iIdx <= iEndVisibleLine; ++iIdx)
                    {
                        try
                        {
                            // Text coloring
                            try
                            {
                                iOffset     = GetCharacterIndexFromLineIndex (iIdx);
                                iOffsetNext = GetCharacterIndexFromLineIndex (iIdx + 1);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                // Fail silently
                            }

                            strLine = Text.Substring (iOffset, iOffsetNext - iOffset);

                            if (strLine.Length >= 101                 &&
                                IsHexColon (strLine.Substring (0, 5)) &&
                                strLine[58] == '<')
                            {
                                //          1         2         3         4         5         6         7         8         9         0         1         2
                                //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                                //0300: 7C 00 52 75  02 CE 71 F5  CE F3 F9 07  D1 F9 0C 6D  <|.Ru ..q. .... ...m>  <@... ...5 .39. J9._>
                                ftDisplayedLines.SetForegroundBrush (m_brPurple,   iOffset,        5); // Data address
                                ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset +  6,  50); // Hex data values
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset +  58,  1); // Lead Op1 angle bracket
                                ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset +  59, 42); // Op1 hex ascii data values
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset +  78,  4); // Middle Op1 angle brackets
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 101,  1); // Trail Op1 angle bracket
                            }
                            //else if (strLine.Length >= 75 &&
                            //         strLine.Substring (0, 4) == "XOXO")
                            //{
                            //    ftDisplayedLines.SetForegroundBrush (m_brPurple,    iOffset,      9);
                            //    ftDisplayedLines.SetForegroundBrush (m_brFuchsia,   iOffset + 18, 9);
                            //    ftDisplayedLines.SetForegroundBrush (m_brDkGreen,   iOffset + 45, 9);
                            //    ftDisplayedLines.SetForegroundBrush (m_brCoral,     iOffset + 63, 5);
                            //}
                            else if (strLine.Length >= 5 &&
                                     IsHexColon (strLine.Substring (14, 5)))
                            {
                                ftDisplayedLines.SetForegroundBrush (m_brBlack,     iOffset,      strLine.Length); // Background text
                                ftDisplayedLines.SetForegroundBrush (m_brRoyalBlue, iOffset,      12);             // Loop / Jump label
                                ftDisplayedLines.SetForegroundBrush (m_brCoral,     iOffset + 14,  5);             // Hex address

                                string strTest1 = strLine.Substring (14, 5); // Hex/Colon test
                                string strTest2 = strLine.Substring (20, 4); // "data" label test
                                if (strTest2 == "data")
                                {
                                    //          1         2         3         4         5         6         7         8         9         0         1         2
                                    //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                                    //              002E: data  7C 3C 40 40  40 40 40 40  <|<@@ @@@@>  <@.       >  <EF__ ____>  <H.'' ''''>
                                    //              001E: data  01 00 6F 03  76 57 1B 5D  <..o. vW.]>  <..?. ...)>  <..01 2345>  <.... ....>
                                    ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset +  26, 25); // Hex data string 
                                    ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset +  52,  1); // Lead data angle bracket
                                    ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset +  53, 48); // Hex, ascii, HPL & 5475 data values
                                    ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset +  62,  4); // First pair angle brackets
                                    ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset +  75,  4); // Second pair angle brackets
                                    ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset +  88,  4); // Third pair angle brackets
                                    ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 101,  1); // Trail data angle bracket
                                }
                                else// if (IsHexColon (strTest1))
                                {
                                    ftDisplayedLines.SetForegroundBrush (m_brMedSeaGrn,    iOffset + 20,  5);                             // Mnemonic
                                    if (strLine.Length >= 26)
                                    {
                                        ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset + 26, Math.Min (strLine.Length, 15)); // Instruction code
                                    }
                                    if (strLine.Length >= 49)
                                    {
                                        ftDisplayedLines.SetForegroundBrush (m_brPurple,  iOffset + 43,  6);                             // Data address
                                    }
                                    if (strLine.Length >= 51)
                                    {
                                        ftDisplayedLines.SetForegroundBrush (m_brDkGreen, iOffset + 51, strLine.Length - 51);            // Annotation
                                    }
                                }
                            }
                            else if (strLine.Length >= 52 &&
                                     IsBlank (strLine.Substring (0, 51)))
                            {
                                    ftDisplayedLines.SetForegroundBrush (m_brDkGreen, iOffset + 51,  strLine.Length - 51); // Annotation
                            }

                            iOffset = GetCharacterIndexFromLineIndex (iIdx);

                            #region Test code for breakpoint and bookmark buttons
                            ////drawingContext.PushClip (new RectangleGeometry (new Rect (0, 0, leftMargin, topMargin))); // Restrict text to textbox
                            //double dLeft = leftMargin - this.HorizontalOffset - 13;
                            //double dTop  = topMargin + (iIdx * 13.55) + 7 - this.VerticalOffset;
                            ////Console.WriteLine ("Left: {0:###.#}  Top: {1:###.#}", dLeft, dTop);
                            //if (iIdx % 3 == 0)
                            //{
                            //    drawingContext.DrawEllipse (new SolidColorBrush (Colors.Red), null,
                            //                                new Point (dLeft, dTop), 5, 5);
                            //}
                            //else if (iIdx % 3 == 1)
                            //{
                            //    drawingContext.DrawEllipse (new SolidColorBrush (Colors.Transparent), new Pen (new SolidColorBrush (Colors.Red), 1.0),
                            //                                new Point (dLeft, dTop), 5, 5);
                            //}

                            //if (iIdx % 4 == 0)
                            //{
                            //    drawingContext.DrawRectangle (new SolidColorBrush (Colors.Blue), null, new Rect (new Point (3, dTop),
                            //                                                                                     new Point (dLeft + 5, dTop + 7)));
                            //}
                            //else if (iIdx % 4 == 1)
                            //{
                            //    drawingContext.DrawRectangle (new SolidColorBrush (Colors.Transparent), new Pen (new SolidColorBrush (Colors.Blue), 1.0),
                            //                                                                           new Rect (new Point (3, dTop),
                            //                                                                                     new Point (dLeft + 5, dTop + 7)));
                            //}
                            ////drawingContext.PushClip (new RectangleGeometry (new Rect (leftMargin, topMargin, this.ActualWidth, this.ActualHeight))); // Restrict text to textbox
                            #endregion

                            #region Highlighting
                            //0         1
                            //01234567890123456789
                            //Entry_0000    0000:
                            if (m_bEnableHighlightLine)
                            {
                                if (iIdx == m_iCurrentLineIdx)
                                {
                                    Geometry geomCurrentLine = ftDisplayedLines.BuildHighlightGeometry (new Point (dLeftMargin, dTopMargin - this.VerticalOffset),
                                                                                                        iOffset + 14, strLine.Length - 14);
                                    if (geomCurrentLine != null)
                                    {
                                        dcRendering.DrawGeometry (m_brYellow, null, geomCurrentLine);
                                    }
                                }
                                else if (m_liGrayedCode.Contains (iIdx)      &&
                                         strLine.Substring (20, 4) != "data" &&
                                         strLine.Substring (0,  5) != "Entry")
                                {
                                    Geometry geomCurrentLine = ftDisplayedLines.BuildHighlightGeometry (new Point (dLeftMargin, dTopMargin - this.VerticalOffset),
                                                                                                        iOffset + 14, 5);
                                    if (geomCurrentLine != null)
                                    {
                                        dcRendering.DrawGeometry (m_brLtGray, null, geomCurrentLine);
                                    }
                                }
                            }
                            #region Old highlighting for breakpoints and bookmarks
                            //if (iIdx == 0)
                            //{
                            //    // TODO: Replace magic numbers with variable
                            //    Geometry geom = ftDisplayedLines.BuildHighlightGeometry (new Point (dLeftMargin, dTopMargin - this.VerticalOffset), iOffset + 12, 2);
                            //    if (geom != null)
                            //    {
                            //        dcRendering.DrawGeometry (m_brBrkPt, null, geom);
                            //    }
                            //}
                            //else if (iIdx == 2)
                            //{
                            //    // TODO: Replace magic numbers with variable
                            //    Geometry geom = ftDisplayedLines.BuildHighlightGeometry (new Point (dLeftMargin, dTopMargin - this.VerticalOffset), iOffset + 12, 2);
                            //    if (geom != null)
                            //    {
                            //        dcRendering.DrawGeometry (m_brDisBrkPt, null, geom);
                            //    }
                            ////}
                            ////else if (iIdx == 2)
                            ////{
                            //    // TODO: Replace magic numbers with variable
                            //    geom = ftDisplayedLines.BuildHighlightGeometry (new Point (dLeftMargin, dTopMargin - this.VerticalOffset), iOffset, 4);
                            //    if (geom != null)
                            //    {
                            //        dcRendering.DrawGeometry (m_brBkMrk, null, geom);
                            //    }
                            //    //geom = ftDisplayedLines.BuildHighlightGeometry (new Point (dLeftMargin, dTopMargin - this.VerticalOffset), iOffset + 4, 4);
                            //    //if (geom != null)
                            //    //{
                            //    //    dcRendering.DrawGeometry (m_brBkMrk, null, geom);
                            //    //}
                            //}
                            //else if (iIdx == 3)
                            //{
                            //    // TODO: Replace magic numbers with variable
                            //    Geometry geom = ftDisplayedLines.BuildHighlightGeometry (new Point (dLeftMargin, dTopMargin - this.VerticalOffset), iOffset + 26, 11);
                            //    if (geom != null)
                            //    {
                            //        dcRendering.DrawGeometry (m_brDisBkMrk, null, geom);
                            //    }
                            //}
                            #endregion
                            #endregion
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            if (strLine.Length > 0)
                            {
                                Console.WriteLine ("ArgumentOutOfRangeException: [" + iIdx.ToString () + "] " + strLine);
                            }
                            // Fail silently
                        }
                    }
                }
                #endregion
                else if (m_eSyntaxColoringMode == SyntaxColoringMode.Trace)
                #region Trace Output
                {
                    iStartVisibleLine = GetFirstVisibleLineIndex ();
                    iEndVisibleLine   = GetLastVisibleLineIndex ();

                    for (int iIdx = iStartVisibleLine; iIdx <= iEndVisibleLine - 1; ++iIdx)
                    {
                        try
                        {
                            try
                            {
                                iOffset     = GetCharacterIndexFromLineIndex (iIdx);
                                iOffsetNext = GetCharacterIndexFromLineIndex (iIdx + 1);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                // Fail silently
                            }

                            strLine = Text.Substring (iOffset, iOffsetNext - iOffset);

                            if (strLine.Length >= 15 &&
                                strLine.Substring (0, 5) == "Step:")
                            {
                                //          1         2         3         4         5         6         7         8         9         0
                                //01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                                //Step: 1,       Timing: 608  608.00µs (6 08)
                                ftDisplayedLines.SetForegroundBrush (m_brTeal,      iOffset,      14);                  // Step Count
                                ftDisplayedLines.SetForegroundBrush (m_brRoyalBlue, iOffset + 15, strLine.Length - 15); // Timing
                            }
                            else if (strLine.Length >= 22 &&
                                     strLine.Substring (0, 3) == "IL:")
                            {
                                //          1         2         3         4         5         6         7         8         9         0
                                //01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                                //IL: M LA    C2 01 0000       0x0000  XR1
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,   iOffset, strLine.Length); // Default for string
                                ftDisplayedLines.SetForegroundBrush (m_brMedSeaGrn, iOffset +  6,  5);        // Mnemonic
                                ftDisplayedLines.SetForegroundBrush (m_brFuchsia,   iOffset + 12, 15);        // Instruction code
                            }
                            else if (strLine.Length >= 20 &&
                                     strLine.Substring (0, 20) == "         1         2")
                            {
                                //          1         2         3         4         5         6         7         8         9         0
                                //01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                                //         1         2         3         4         5         6         7         8         9      
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen, iOffset, strLine.Length);
                            }
                            else if (strLine.Length >= 20 &&
                                     strLine.Substring (0, 20) == "12345678901234567890")
                            {
                                //          1         2         3         4         5         6         7         8         9         0
                                //01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                                //123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen, iOffset, strLine.Length);
                            }
                            else if (strLine.Length >= 126                  &&
                                     IsHexColon (strLine.Substring (25, 5)) &&
                                     strLine[83] == '<')
                            {
                                //          1         2         3         4         5         6         7         8         9         0         1         2
                                //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                                //                         0080: 40 40 40 40  40 40 40 40  40 40 40 40  40 40 40 40  <@@@@ @@@@ @@@@ @@@@>  <                   >
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset,  24);
                                ftDisplayedLines.SetForegroundBrush (m_brPurple,   iOffset + 25,   5); // Data address
                                ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset + 31,  50); // Hex data values
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 83,   1); // Lead Op1 angle bracket
                                ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset + 84,  42); // Op1 hex ascii data values
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 103,  4); // Middle Op1 angle brackets
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 126,  1); // Trail Op1 angle bracket
                            }
                            else if (strLine.Length >= 9 &&
                                     strLine.Substring (0, 9) == "   >>>   ")
                            {
                                //          1         2         3         4         5         6         7         8         9         0
                                //01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                                //   >>>   MFCU Read Buffer (0x0000):
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen, iOffset, strLine.Length);
                            }
                            else if (strLine.Length >= 17 &&
                                     strLine.Substring (0, 17) == "            MFCU ")
                            {
                                //          1         2         3         4         5         6         7         8         9         0
                                //01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                                //            MFCU Read DAR after LIO: 0x0000
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen, iOffset, strLine.Length);
                            }
                            else if (strLine.Length >= 106                 &&
                                     IsHexColon (strLine.Substring (5, 5)) &&
                                     strLine[63] == '<')
                            {
                                //          1         2         3         4         5         6         7         8         9         0
                                //01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                                //Op2: 0020: 71 F5 FF F3  F1 40 F1 F1  00 D0 00 00               <q... .@.. ....     >  <.5.3 1 11 .}..     >
                                ftDisplayedLines.SetForegroundBrush (m_brPurple,   iOffset,        4); // "Op1:" label 
                                ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset +  5,  57); // Hex data values
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 63,   1); // Lead Op1 angle bracket
                                ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset + 64,  42); // Op1 hex ascii data values
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 83,   4); // Middle Op1 angle brackets
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 106,  1); // Trail Op1 angle bracket
                            }
                            else if (strLine.Length >= 102                 &&
                                     IsHexColon (strLine.Substring (0, 5)) &&
                                     strLine[58] == '<')
                            {
                                //          1         2         3         4         5         6         7         8         9         0
                                //01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                                //0000: 5C 09 71 14  5C 14 86 34  D0 00 F4 5F  88 F2 04 03  <\\.q. \\..4 ..._ ....>  <*... *.f. }.4^ h2..>
                                ftDisplayedLines.SetForegroundBrush (m_brPurple,   iOffset,        5); // Hex data address
                                ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset + 6,   51); // Hex data values
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 58,   1); // Lead Op1 angle bracket
                                ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset + 59,  42); // Op1 hex ascii data values
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 78,   4); // Middle Op1 angle brackets
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 101,  1); // Trail Op1 angle bracket
                            }
                            else if (strLine.Length >= 61 &&
                                     strLine.Substring (0, 4) == "Op1:")
                            {
                                //          1         2         3         4         5         6         7         8         9         0         1         2
                                //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                                //Op1: 0060: 00 00 00 00  00 00 00 00  <.... ....>  <.... ....>  Op2: 002C: F0 7C 6C D0  87 60 57 20  <.|l. .`W >  <0@%} g-..>
                                ftDisplayedLines.SetForegroundBrush (m_brPurple,   iOffset,       4); // "Op1:" label 
                                ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset + 5,  31); // Hex data values
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 37,  1); // Lead Op1 angle bracket
                                ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset + 38, 22); // Op1 hex ascii data values
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 47,  4); // Middle Op1 angle brackets
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 60,  1); // Trail Op1 angle bracket
                                if (strLine.Contains ("Op2:"))
                                {
                                    ftDisplayedLines.SetForegroundBrush (m_brPurple,   iOffset +  63,  4); // "Op2:" label
                                    ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 100,  1); // Lead Op2 angle bracket
                                    ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset +  68, 31); // Op2 hex data values
                                    ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset + 101, 22); // Op2 ascii data values
                                    ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 110,  4); // Middle Op2 angle brackets
                                    ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 123,  1); // Trail Op2 angle bracket
                                }
                            }
                            else if (strLine.Length >= 118                 &&
                                     IsHexColon (strLine.Substring (0, 5)) &&
                                     strLine.Substring (43, 4) == "IAR:")
                            {
                                //          1         2         3         4         5         6         7         8         9         0         1         2
                                //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                                //0000: LA    C2 01 0000       00 00         IAR: 0004  XR1: 0000  XR2: 0000  ARR: 0000  CR: EQ            Timing: 6.08µs
                                //00D5: A     76 02 FA,1       00 FA         IAR: 00D8  XR1: 0000  XR2: 0053  ARR: 00A6  CR: HI       BO XR2: 0x0057 --> 0x0053
                                ftDisplayedLines.SetForegroundBrush (m_brCoral,     iOffset,        5); // Instruction address
                                ftDisplayedLines.SetForegroundBrush (m_brMedSeaGrn, iOffset +   6,  5); // Mnemonic
                                ftDisplayedLines.SetForegroundBrush (m_brFuchsia,   iOffset +  12, 15); // Instruction code
                                ftDisplayedLines.SetForegroundBrush (m_brDkGreen,   iOffset +  29, 13); // Annotation
                                ftDisplayedLines.SetForegroundBrush (m_brPurple,    iOffset +  43,  4); // IAR label
                                ftDisplayedLines.SetForegroundBrush (m_brFuchsia,   iOffset +  48, 55); // Register values
                                ftDisplayedLines.SetForegroundBrush (m_brPurple,    iOffset +  54,  4); // XR1 label
                                ftDisplayedLines.SetForegroundBrush (m_brPurple,    iOffset +  65,  4); // XR2 label
                                ftDisplayedLines.SetForegroundBrush (m_brPurple,    iOffset +  76,  4); // ARR label
                                ftDisplayedLines.SetForegroundBrush (m_brPurple,    iOffset +  87,  3); // CR label
                                if (strLine.Substring (105, 8) == "Timing: ")
                                {
                                    ftDisplayedLines.SetForegroundBrush (m_brPurple, iOffset + 105, 15); // Timing
                                }
                                else
                                {
                                    ftDisplayedLines.SetForegroundBrush (m_brPurple,   iOffset + 103, 4); // Register label
                                    ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset + 108, 6); // Old register value
                                    ftDisplayedLines.SetForegroundBrush (m_brDkGreen,  iOffset + 115, 3); // "-->"
                                    ftDisplayedLines.SetForegroundBrush (m_brFuchsia,  iOffset + 119, 6); // New register value
                                }
                            }
                            else if (strLine.Length >= 7 &&
                                     strLine.Substring (0, 7) == "- - - -")
                            {
                                ftDisplayedLines.SetForegroundBrush (m_brFirebrick, iOffset, strLine.Length);
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            //Console.WriteLine ("ArgumentOutOfRangeException: [" + iIdx.ToString () + "] " + strLine);
                            // Fail silently
                        }
                    }
                }
                #endregion // Trace Output

                dcRendering.DrawText (ftDisplayedLines, new Point (dLeftMargin - this.HorizontalOffset, dTopMargin - this.VerticalOffset));
                #endregion // Text Coloring and Shading
            }
        }

        private bool IsHexColon (string strTest)
        {
            if (strTest.Length < 5 ||
                strTest[4] != ':')
            {
                return false;
            }

            for (int iIdx = 0; iIdx < 4; ++iIdx)
            {
                char cTest = strTest.ToLower ()[iIdx];
                if (!((cTest >= '0' && cTest <= '9') ||
                      (cTest >= 'a' && cTest <= 'f')))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsBlank (string strTest)
        {
            foreach (char cTest in strTest)
            {
                if (cTest != ' ')
                {
                    return false;
                }
            }

            return true;
        }

        private void EnsureScrolling ()
        {
            if (!m_bScrollingEventEnabled)
            {
                DependencyObject dp = VisualTreeHelper.GetChild (this, 0);
                //dp = VisualTreeHelper.GetChild (dp, 0);
                ScrollViewer sv = VisualTreeHelper.GetChild (dp, 0) as ScrollViewer;
                //ScrollViewer sv = dp as ScrollViewer; // Fix for screen flickering 2012-04-08
                sv.ScrollChanged += new ScrollChangedEventHandler (ScrollChanged);
                m_bScrollingEventEnabled = true;
            }
        }

        //#region LineNumber Properties
        //public static DependencyProperty ShowLineNumbersProperty = DependencyProperty.Register ("ShowLineNumbers", typeof (bool), typeof (CodeBox),
        //              new FrameworkPropertyMetadata (true, FrameworkPropertyMetadataOptions.AffectsRender));
        //[Category ("LineNumbers")]
        //public bool ShowLineNumbers
        //{
        //    get { return (bool)GetValue (ShowLineNumbersProperty); }
        //    set { SetValue (ShowLineNumbersProperty, value); }
        //}

        //public static DependencyProperty LineNumberForegroundProperty = DependencyProperty.Register ("LineNumberForeground", typeof (Brush), typeof (CodeBox),
        //              new FrameworkPropertyMetadata (new SolidColorBrush (Colors.Gray), FrameworkPropertyMetadataOptions.AffectsRender));
        //[Category ("LineNumbers")]
        //public Brush LineNumberForeground
        //{
        //    get { return (Brush)GetValue (LineNumberForegroundProperty); }
        //    set { SetValue (LineNumberForegroundProperty, value); }
        //}

        //public static DependencyProperty LineNumberMarginWidthProperty = DependencyProperty.Register ("LineNumberMarginWidth", typeof (double), typeof (CodeBox),
        //              new FrameworkPropertyMetadata (15.0, FrameworkPropertyMetadataOptions.AffectsRender));
        //[Category ("LineNumbers")]
        //public double LineNumberMarginWidth
        //{
        //    get { return (Double)GetValue (LineNumberMarginWidthProperty); }
        //    set { SetValue (LineNumberMarginWidthProperty, value); }
        //}

        //public static DependencyProperty StartingLineNumberProperty = DependencyProperty.Register ("StartingLineNumber", typeof (int), typeof (CodeBox),
        //              new FrameworkPropertyMetadata (1, FrameworkPropertyMetadataOptions.AffectsRender));
        //[Category ("LineNumbers")]
        //public int StartingLineNumber
        //{
        //    get { return (int)GetValue (StartingLineNumberProperty); }
        //    set { SetValue (StartingLineNumberProperty, value); }
        //}
        //#endregion

        private void ScrollChanged (object sender, ScrollChangedEventArgs e)
        {
            this.InvalidateVisual ();
        }
    }
}
