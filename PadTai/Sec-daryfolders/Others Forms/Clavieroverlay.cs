using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using PadTai.Classes.Others;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using PadTai.Classes.Controlsdesign;
using System.Runtime.InteropServices;


namespace PadTai.Sec_daryfolders.Others
{
    public partial class Clavieroverlay : Form
    {
        public event EventHandler EnterButtonClicked;
        private DraggableForm draggableForm;
        private FontResizer fontResizer;
        private Control _targetControl;
        private ControlResizer resizer;
        private Timer backspaceTimer;
        bool capState = false;


        public Clavieroverlay(Control targetControl)
        {
            InitializeComponent();

            _targetControl = targetControl;

            InitializeControlResizer();
            this.DoubleBuffered = true; 
            UpdateButtonText();
            InitializeBackspaceButton();
            LocalizeControls();
            CenterLabel();
            ApplyTheme();
        }

        #region Resizers
        private void InitializeControlResizer()
        {
            roundedPanel2.Visible = false;

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
            draggableForm = new DraggableForm();
            draggableForm.EnableDragging(this, this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel1);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(button3);
            resizer.RegisterControl(button4);
            resizer.RegisterControl(button5);
            resizer.RegisterControl(button6);
            resizer.RegisterControl(button7);
            resizer.RegisterControl(buttonA);
            resizer.RegisterControl(buttonB);
            resizer.RegisterControl(buttonC);
            resizer.RegisterControl(buttonD);
            resizer.RegisterControl(buttonE);
            resizer.RegisterControl(buttonF);
            resizer.RegisterControl(buttonG);
            resizer.RegisterControl(buttonH);
            resizer.RegisterControl(buttonI);
            resizer.RegisterControl(buttonJ);
            resizer.RegisterControl(buttonK);
            resizer.RegisterControl(buttonL);
            resizer.RegisterControl(buttonM);
            resizer.RegisterControl(buttonN);
            resizer.RegisterControl(buttonO);
            resizer.RegisterControl(buttonP);
            resizer.RegisterControl(buttonQ);
            resizer.RegisterControl(buttonR);
            resizer.RegisterControl(buttonS);
            resizer.RegisterControl(buttonT);
            resizer.RegisterControl(buttonU);
            resizer.RegisterControl(buttonV);
            resizer.RegisterControl(buttonW);
            resizer.RegisterControl(buttonX);
            resizer.RegisterControl(buttonY);
            resizer.RegisterControl(buttonZ);
            resizer.RegisterControl(buttonF1);
            resizer.RegisterControl(buttonF2);
            resizer.RegisterControl(buttonF3);
            resizer.RegisterControl(buttonF4);
            resizer.RegisterControl(buttonF5);
            resizer.RegisterControl(buttonF6);
            resizer.RegisterControl(buttonF7);
            resizer.RegisterControl(buttonF8);
            resizer.RegisterControl(buttonF9);
            resizer.RegisterControl(buttonLT);
            resizer.RegisterControl(buttonGT);
            resizer.RegisterControl(buttonWin);
            resizer.RegisterControl(buttonEnd);
            resizer.RegisterControl(buttonF10);
            resizer.RegisterControl(buttonF11);
            resizer.RegisterControl(buttonF12);
            resizer.RegisterControl(buttonESC);           
            resizer.RegisterControl(buttonTab);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(buttonLAlt);
            resizer.RegisterControl(buttonRAlt);
            resizer.RegisterControl(buttonHome);
            resizer.RegisterControl(buttonNum0);
            resizer.RegisterControl(buttonNum1);
            resizer.RegisterControl(buttonNum2);
            resizer.RegisterControl(buttonNum3);
            resizer.RegisterControl(buttonNum4);
            resizer.RegisterControl(buttonNum5);
            resizer.RegisterControl(buttonNum6);
            resizer.RegisterControl(buttonNum7);
            resizer.RegisterControl(buttonNum8);
            resizer.RegisterControl(buttonNum9);
            resizer.RegisterControl(buttonMenu);
            resizer.RegisterControl(buttonRAlt);
            resizer.RegisterControl(buttonSpace);
            resizer.RegisterControl(buttonQuote);
            resizer.RegisterControl(buttonEnter);
            resizer.RegisterControl(buttonLCtrl);
            resizer.RegisterControl(buttonSCRLK);
            resizer.RegisterControl(buttonBREAK);
            resizer.RegisterControl(buttonTilde);            
            resizer.RegisterControl(buttonRCtrl);
            resizer.RegisterControl(buttonDelete);
            resizer.RegisterControl(buttonRShift);
            resizer.RegisterControl(buttonLShift);
            resizer.RegisterControl(buttonPageUp);
            resizer.RegisterControl(buttonInsert);
            resizer.RegisterControl(buttonPRTSCR);
            resizer.RegisterControl(buttonNumMin);
            resizer.RegisterControl(buttonArrowUp);
            resizer.RegisterControl(buttonNumPlus);
            resizer.RegisterControl(buttonSetting);
            resizer.RegisterControl(buttonPageDown);
            resizer.RegisterControl(buttonCapsLock);
            resizer.RegisterControl(buttonSemiColon);
            resizer.RegisterControl(buttonArrowLeft);
            resizer.RegisterControl(buttonArrowDown);
            resizer.RegisterControl(buttonBackSlash);            
            resizer.RegisterControl(buttonLeftBrace);
            resizer.RegisterControl(buttonBackspace);
            resizer.RegisterControl(buttonRightBrace);
            resizer.RegisterControl(buttonArrowRight);
            resizer.RegisterControl(buttonForwardSlash);
        }
        #endregion

        #region Keystrokes

        private void buttonESC_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{ESC}");
        }

        private void buttonF1_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{F1}");
        }

        private void buttonF2_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{F2}");
        }

        private void buttonF3_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{F3}");
        }

        private void buttonF4_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{F4}");
        }

        private void buttonF5_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{F5}");
        }

        private void buttonF6_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{F6}");
        }

        private void buttonF7_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{F7}");
        }

        private void buttonF8_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{F8}");
        }

        private void buttonF9_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{F9}");
        }

        private void buttonF10_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{F10}");
        }

        private void buttonF11_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{F11}");
        }

        private void buttonF12_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{F12}");
        }

        private void buttonPRTSCR_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{PRTSC}");
        }

        private void buttonSCRLK_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{SCROLLLOCK}");
        }

        private void buttonBREAK_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{BREAK}");
        }

        private void buttonTilde_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{`}" : "{~}");
            AppendTextToTargetControl(buttonTilde.Text);
        }

        private void buttonNum1_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{1}" : "{!}");
            AppendTextToTargetControl(buttonNum1.Text);
        }

        private void buttonNum2_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{2}" : "{@}");
            AppendTextToTargetControl(buttonNum2.Text);
        }

        private void buttonNum3_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{3}" : "{#}");
            AppendTextToTargetControl(buttonNum3.Text);
        }

        private void buttonNum4_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{4}" : "{$}");
            AppendTextToTargetControl(buttonNum4.Text);
        }

        private void buttonNum5_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{5}" : "{%}");
            AppendTextToTargetControl(buttonNum5.Text);
        }

        private void buttonNum6_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{6}" : "{^}");
            AppendTextToTargetControl(buttonNum6.Text);
        }

        private void buttonNum7_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{7}" : "{&}");
            AppendTextToTargetControl(buttonNum7.Text);
        }

        private void buttonNum8_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{8}" : "{*}");
            AppendTextToTargetControl(buttonNum8.Text);
        }

        private void buttonNum9_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{9}" : "{(}");
            AppendTextToTargetControl(buttonNum9.Text);
        }

        private void buttonNum0_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{0}" : "{)}");
            AppendTextToTargetControl(buttonNum0.Text);
        }

        private void buttonNumMin_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{-}" : "{_}");
            AppendTextToTargetControl(buttonNumMin.Text);
        }

        private void buttonNumPlus_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{=}" : "{+}");
            AppendTextToTargetControl(buttonNumPlus.Text);
        }

        private void buttonBackspace_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{BACKSPACE}");

            if (_targetControl is TextBox textBox && textBox.ReadOnly == false)
            {
                if (textBox.SelectionLength > 0)
                {
                    int selectionStart = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Remove(selectionStart, textBox.SelectionLength);
                    textBox.SelectionStart = selectionStart; 
                }
                else if (textBox.SelectionStart > 0) 
                {
                    int selectionStart = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Remove(selectionStart - 1, 1);
                    textBox.SelectionStart = selectionStart - 1; 
                }
            }
            else if (_targetControl is RichTextBox richTextBox && richTextBox.ReadOnly == false)
            {
                if (richTextBox.SelectionLength > 0)
                {
                    int selectionStart = richTextBox.SelectionStart;
                    richTextBox.Text = richTextBox.Text.Remove(selectionStart, richTextBox.SelectionLength);
                    richTextBox.SelectionStart = selectionStart; 
                }
                else if (richTextBox.SelectionStart > 0) 
                {
                    int selectionStart = richTextBox.SelectionStart;
                    richTextBox.Text = richTextBox.Text.Remove(selectionStart - 1, 1);
                    richTextBox.SelectionStart = selectionStart - 1; 
                }
            }
            else if (_targetControl is RoundedTextbox roundTextBox && roundTextBox.ReadOnly == false)
            {
                if (roundTextBox.SelectionLength > 0)
                {
                    int selectionStart = roundTextBox.SelectionStart;
                    roundTextBox.Text = roundTextBox.Text.Remove(selectionStart, roundTextBox.SelectionLength);
                    roundTextBox.SelectionStart = selectionStart; 
                }
                else if (roundTextBox.SelectionStart > 0) 
                {
                    int selectionStart = roundTextBox.SelectionStart;
                    roundTextBox.Text = roundTextBox.Text.Remove(selectionStart - 1, 1);
                    roundTextBox.SelectionStart = selectionStart - 1; 
                }
            }
            else if (_targetControl is customBox customBox && customBox.DropDownStyle == ComboBoxStyle.DropDown)
            {
                if (customBox.SelectionLength > 0)
                {
                    int selectionStart = customBox.SelectionStart;
                    customBox.Text = customBox.Text.Remove(selectionStart, customBox.SelectionLength);
                    customBox.SelectionStart = selectionStart; 
                }
                else if (customBox.SelectionStart > 0) 
                {
                    int selectionStart = customBox.SelectionStart;
                    customBox.Text = customBox.Text.Remove(selectionStart - 1, 1);
                    customBox.SelectionStart = selectionStart - 1;
                }
            }
        }

        private void InitializeBackspaceButton()
        {
            backspaceTimer = new Timer();
            backspaceTimer.Interval = 100;
            backspaceTimer.Tick += BackspaceTimer_Tick;
        }

        private void buttonBackspace_MouseDown(object sender, MouseEventArgs e)
        {
            backspaceTimer.Start();
        }

        private void buttonBackspace_MouseUp(object sender, MouseEventArgs e)
        {
            backspaceTimer.Stop();
        }

        private void BackspaceTimer_Tick(object sender, EventArgs e)
        {
            if (_targetControl is TextBox textBox && textBox.ReadOnly == false)
            {
                if (textBox.SelectionLength > 0)
                {
                    int selectionStart = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Remove(selectionStart, textBox.SelectionLength);
                    textBox.SelectionStart = selectionStart;
                }
                else if (textBox.SelectionStart > 0)
                {
                    int selectionStart = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Remove(selectionStart - 1, 1);
                    textBox.SelectionStart = selectionStart - 1;
                }
            }
            else if (_targetControl is RichTextBox richTextBox && richTextBox.ReadOnly == false)
            {
                if (richTextBox.SelectionLength > 0)
                {
                    int selectionStart = richTextBox.SelectionStart;
                    richTextBox.Text = richTextBox.Text.Remove(selectionStart, richTextBox.SelectionLength);
                    richTextBox.SelectionStart = selectionStart;
                }
                else if (richTextBox.SelectionStart > 0)
                {
                    int selectionStart = richTextBox.SelectionStart;
                    richTextBox.Text = richTextBox.Text.Remove(selectionStart - 1, 1);
                    richTextBox.SelectionStart = selectionStart - 1;
                }
            }
            else if (_targetControl is RoundedTextbox roundTextBox && roundTextBox.ReadOnly == false)
            {
                if (roundTextBox.SelectionLength > 0)
                {
                    int selectionStart = roundTextBox.SelectionStart;
                    roundTextBox.Text = roundTextBox.Text.Remove(selectionStart, roundTextBox.SelectionLength);
                    roundTextBox.SelectionStart = selectionStart;
                }
                else if (roundTextBox.SelectionStart > 0)
                {
                    int selectionStart = roundTextBox.SelectionStart;
                    roundTextBox.Text = roundTextBox.Text.Remove(selectionStart - 1, 1);
                    roundTextBox.SelectionStart = selectionStart - 1;
                }
            }
            else if (_targetControl is customBox customBox && customBox.DropDownStyle == ComboBoxStyle.DropDown)
            {
                if (customBox.SelectionLength > 0)
                {
                    int selectionStart = customBox.SelectionStart;
                    customBox.Text = customBox.Text.Remove(selectionStart, customBox.SelectionLength);
                    customBox.SelectionStart = selectionStart;
                }
                else if (customBox.SelectionStart > 0)
                {
                    int selectionStart = customBox.SelectionStart;
                    customBox.Text = customBox.Text.Remove(selectionStart - 1, 1);
                    customBox.SelectionStart = selectionStart - 1;
                }
            }
        }


        private void buttonInsert_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{INSERT}");
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{HOME}");
        }

        private void buttonPageUp_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{PGUP}");
        }

        private void buttonPageDown_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{PGDN}");
        }

        private void buttonEnd_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{END}");
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{DELETE}");
        }

        private void buttonTab_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{TAB}");
        }

        private void buttonQ_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{q}" : "{Q}");
            AppendTextToTargetControl(buttonQ.Text);
        }

        private void buttonW_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{w}" : "{W}");
            AppendTextToTargetControl(buttonW.Text);
        }

        private void buttonE_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{e}" : "{E}");
            AppendTextToTargetControl(buttonE.Text);
        }

        private void buttonR_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{r}" : "{R}");
            AppendTextToTargetControl(buttonR.Text);
        }

        private void buttonT_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{t}" : "{T}");
            AppendTextToTargetControl(buttonT.Text);
        }

        private void buttonY_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{y}" : "{Y}");
            AppendTextToTargetControl(buttonY.Text);
        }

        private void buttonU_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{u}" : "{U}");
            AppendTextToTargetControl(buttonU.Text);
        }

        private void buttonI_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{i}" : "{I}");
            AppendTextToTargetControl(buttonI.Text);
        }

        private void buttonO_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{o}" : "{O}");
            AppendTextToTargetControl(buttonO.Text);
        }

        private void buttonP_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{p}" : "{P}");
            AppendTextToTargetControl(buttonP.Text);
        }

        private void buttonLeftBrace_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{[}" : "{{}");
            AppendTextToTargetControl(buttonLeftBrace.Text);
        }

        private void buttonRightBrace_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{]}" : "{}}");
            AppendTextToTargetControl(buttonRightBrace.Text);
        }

        private void buttonBackSlash_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? @"{\}" : @"{|}");
            AppendTextToTargetControl(buttonBackSlash.Text);
        }

        private void buttonCapsLock_Click(object sender, EventArgs e)
        {
            capState = !capState;
            UpdateButtonText();  

            if (capState)
            {
                buttonCapsLock.BackColor = Color.Salmon;
            }
            else
            {
                buttonCapsLock.BackColor = SystemColors.Highlight;
            }
        }

        private void buttonA_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{a}" : "{A}");
            AppendTextToTargetControl(buttonA.Text);
        }

        private void buttonS_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{s}" : "{S}");
            AppendTextToTargetControl(buttonS.Text);
        }

        private void buttonD_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{d}" : "{D}");
            AppendTextToTargetControl(buttonD.Text);
        }

        private void buttonF_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{f}" : "{F}");
            AppendTextToTargetControl(buttonF.Text);
        }

        private void buttonG_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{g}" : "{G}");
            AppendTextToTargetControl(buttonG.Text);
        }

        private void buttonH_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{h}" : "{H}");
            AppendTextToTargetControl(buttonH.Text);
        }

        private void buttonJ_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{j}" : "{J}");
            AppendTextToTargetControl(buttonJ.Text);
        }

        private void buttonK_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{k}" : "{K}");
            AppendTextToTargetControl(buttonK.Text);
        }

        private void buttonL_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{l}" : "{L}");
            AppendTextToTargetControl(buttonL.Text);
        }

        private void buttonSemiColon_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? @"{;}" : @"{:}");
            AppendTextToTargetControl(buttonSemiColon.Text);
        }

        private void buttonQuote_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{\'}" : "{\"}");
            AppendTextToTargetControl(buttonQuote.Text);
        }

        private void buttonEnter_Click(object sender, EventArgs e)
        {             
             EnterButtonClicked?.Invoke(this, EventArgs.Empty);    
             SendKeys.Send("{ENTER}");
        }

        private void buttonLShift_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("+");
        }

        private void buttonRShift_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("+");
        }

        private void buttonZ_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{z}" : "{Z}");
            AppendTextToTargetControl(buttonZ.Text);
        }

        private void buttonX_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{x}" : "{X}");
            AppendTextToTargetControl(buttonX.Text);
        }

        private void buttonC_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{c}" : "{C}");
            AppendTextToTargetControl(buttonC.Text);
        }

        private void buttonV_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{v}" : "{V}");
            AppendTextToTargetControl(buttonV.Text);
        }

        private void buttonB_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{b}" : "{B}");
            AppendTextToTargetControl(buttonB.Text);
        }

        private void buttonN_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{n}" : "{N}");
            AppendTextToTargetControl(buttonN.Text);
        }

        private void buttonM_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? "{m}" : "{M}");
            AppendTextToTargetControl(buttonM.Text);
        }

        private void buttonGT_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? @"{,}" : @"{<}");
            AppendTextToTargetControl(buttonGT.Text);
        }

        private void buttonLT_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? @"{.}" : @"{>}");
            AppendTextToTargetControl(buttonLT.Text);
        }

        private void buttonForwardSlash_Click(object sender, EventArgs e)
        {
            SendKeys.Send(capState.Equals(false) ? @"{/}" : @"{?}");
            AppendTextToTargetControl(buttonForwardSlash.Text);
        }

        private void buttonLCtrl_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^");
        }

        private void buttonWin_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^({ESC})");
        }

        private void buttonLAlt_Click(object sender, EventArgs e)
        {
            SendKeys.Send("%");
        }

        private void buttonSpace_Click(object sender, EventArgs e)
        {
            if(_targetControl != null) 
            {
                AppendTextToTargetControl(" ");
            }
            else 
            {
                SendKeys.Send(" ");
            }
        }

        private void buttonRAlt_Click(object sender, EventArgs e)
        {
            SendKeys.Send("%");
        }

        private void buttonSetting_Click(object sender, EventArgs e)
        {
            if (roundedPanel2.Visible == false)
            {
                roundedPanel2.Visible = true;
            }
            else 
            {
                roundedPanel2.Visible = false;
            }
        }

        private void buttonMenu_Click(object sender, EventArgs e)
        {
            SendKeys.Send("+({F10})");
        }

        private void buttonRCtrl_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^");
        }

        private void buttonArrowUp_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{UP}");
        }

        private void buttonArrowLeft_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{LEFT}");
            MoveCursorLeft();
        }

        private void buttonArrowDown_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{DOWN}");
        }

        private void buttonArrowRight_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{RIGHT}");
            MoveCursorRight();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            AppendTextToTargetControl(button1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AppendTextToTargetControl(button2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AppendTextToTargetControl(button3.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AppendTextToTargetControl(button4.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AppendTextToTargetControl(button5.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            AppendTextToTargetControl(button6.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            AppendTextToTargetControl(button7.Text);
        }

        private void AppendTextToTargetControl(string buttonText)
        {
            string textToAppend = capState ? buttonText.ToUpper() : buttonText.ToLower();
            AppendToControl(textToAppend);
            roundedPanel2.Visible = false;
        }

        private void AppendToControl(string textToAppend)
        {
            if (_targetControl is TextBox textBox && textBox.ReadOnly == false)
            {
                textBox.AppendText(textToAppend);
            }
            else if (_targetControl is RichTextBox richTextBox && richTextBox.ReadOnly == false)
            {
                richTextBox.AppendText(textToAppend);
            }
            else if (_targetControl is RoundedTextbox roundtextBox && roundtextBox.ReadOnly == false)
            {
                roundtextBox.AppendText(textToAppend);
            }
            else if (_targetControl is customBox customBox && customBox.DropDownStyle == ComboBoxStyle.DropDown)
            {
                if (customBox.DropDownStyle == ComboBoxStyle.DropDown || customBox.DropDownStyle == ComboBoxStyle.DropDownList)
                {
                    customBox.AppendText(textToAppend);
                }
            }
        }

        private void MoveCursorLeft()
        {
            if (_targetControl is TextBox textBox)
            {
                if (textBox.SelectionStart > 0)
                {
                    textBox.SelectionStart--; 
                }
            }
            else if (_targetControl is RichTextBox richTextBox)
            {
                if (richTextBox.SelectionStart > 0)
                {
                    richTextBox.SelectionStart--; 
                }
            }
            else if (_targetControl is RoundedTextbox roundtextBox)
            {
                if (roundtextBox.SelectionStart > 0)
                {
                    roundtextBox.SelectionStart--;
                }
            }
            else if (_targetControl is customBox customBox)
            {
                if (customBox.SelectionStart > 0)
                {
                    customBox.SelectionStart--;
                }
            }
        }

        private void MoveCursorRight()
        {
            if (_targetControl is TextBox textBox)
            {
                if (textBox.SelectionStart < textBox.Text.Length)
                {
                    textBox.SelectionStart++; 
                }
            }
            else if (_targetControl is RichTextBox richTextBox)
            {
                if (richTextBox.SelectionStart < richTextBox.Text.Length)
                {
                    richTextBox.SelectionStart++; 
                }
            }
            else if (_targetControl is RoundedTextbox roundtextBox)
            {
                if (roundtextBox.SelectionStart < roundtextBox.Text.Length)
                {
                    roundtextBox.SelectionStart++; 
                }
            }
            else if (_targetControl is customBox customBox)
            {
                if (customBox.SelectionStart < customBox.Text.Length)
                {
                    customBox.SelectionStart++; 
                }
            }
        }

        private void UpdateButtonText()
        {
            buttonTilde.Text = capState ? "~" : "`";
            buttonNum1.Text = capState ? "!" : "1";
            buttonNum2.Text = capState ? "@" : "2";
            buttonNum3.Text = capState ? "#" : "3";
            buttonNum4.Text = capState ? "$" : "4";
            buttonNum5.Text = capState ? "%" : "5";
            buttonNum6.Text = capState ? "^" : "6";
            buttonNum7.Text = capState ? "&" : "7";
            buttonNum8.Text = capState ? "*" : "8";
            buttonNum9.Text = capState ? "(" : "9";
            buttonNum0.Text = capState ? ")" : "0";
            buttonNumMin.Text = capState ? "_" : "-";
            buttonNumPlus.Text = capState ? "+" : "=";
            buttonLeftBrace.Text = capState ? "{" : "[";
            buttonRightBrace.Text = capState ? "}" : "]";
            buttonBackSlash.Text = capState ? "|" : "\\";
            buttonSemiColon.Text = capState ? ":" : ";";
            buttonQuote.Text = capState ? "\"" : "'";
            buttonGT.Text = capState ? "<" : ",";
            buttonLT.Text = capState ? ">" : ".";
            buttonForwardSlash.Text = capState ? "?" : "/";
        }

        private void UpdateButtonText(string language)
        {
            switch (language)
            {
                case "English":
                    buttonA.Text = "A";
                    buttonB.Text = "B";
                    buttonC.Text = "C";
                    buttonD.Text = "D";
                    buttonE.Text = "E";
                    buttonF.Text = "F";
                    buttonG.Text = "G";
                    buttonH.Text = "H";
                    buttonI.Text = "I";
                    buttonJ.Text = "J";
                    buttonK.Text = "K";
                    buttonL.Text = "L";
                    buttonM.Text = "M";
                    buttonN.Text = "N";
                    buttonO.Text = "O";
                    buttonP.Text = "P";
                    buttonQ.Text = "Q";
                    buttonR.Text = "R";
                    buttonS.Text = "S";
                    buttonT.Text = "T";
                    buttonU.Text = "U";
                    buttonV.Text = "V";
                    buttonW.Text = "W";
                    buttonX.Text = "X";
                    buttonY.Text = "Y";
                    buttonZ.Text = "Z";
                    break;

                case "Русский":
                    buttonA.Text = "Ф";
                    buttonB.Text = "И";
                    buttonC.Text = "С";
                    buttonD.Text = "В";
                    buttonE.Text = "У";
                    buttonF.Text = "А";
                    buttonG.Text = "П";
                    buttonH.Text = "Р";
                    buttonI.Text = "Ш";
                    buttonJ.Text = "О";
                    buttonK.Text = "Л";
                    buttonL.Text = "Д";
                    buttonM.Text = "Ь";
                    buttonN.Text = "Т";
                    buttonO.Text = "Щ";
                    buttonP.Text = "З";
                    buttonQ.Text = "Й";
                    buttonR.Text = "К";
                    buttonS.Text = "Ы";
                    buttonT.Text = "Е";
                    buttonU.Text = "Г";
                    buttonV.Text = "М";
                    buttonW.Text = "Ц";
                    buttonX.Text = "Ч";
                    buttonY.Text = "Н";
                    buttonZ.Text = "Я";
                    break;

                default:
                    break;
            }
        } 

        private void rjButton3_Click(object sender, EventArgs e)
        {
            UpdateButtonText("English");
            roundedPanel2.Visible = false;
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            UpdateButtonText("Русский");
            roundedPanel2.Visible = false;
        }

        private void rjButton4_Click(object sender, EventArgs e)
        {
            UpdateButtonText("English");
            roundedPanel2.Visible = false;
        }

        private void CenterLabel()
        {
            label1.Left = (roundedPanel2.ClientSize.Width - label1.Width) / 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide(); 
        }

        // Active window
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ExStyle |= 0x08000000;
                return param;
            }
        }
        #endregion

        public void boardLocationBottom()
        {
            if (Properties.Settings.Default.isKeyboardFullScreen)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width, this.Height);
                int horizontalCenter = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
                this.Location = new Point(horizontalCenter, Screen.PrimaryScreen.Bounds.Height - this.Height);
            }
            else 
            {
                this.StartPosition = FormStartPosition.Manual;
                int horizontalCenter = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
                this.Location = new Point(horizontalCenter, Screen.PrimaryScreen.Bounds.Height - this.Height);
            }
        }

        public void boardLocationTop()
        {
            if (Properties.Settings.Default.isKeyboardFullScreen)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width, this.Height);
                int horizontalCenter = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
                this.Location = new Point(horizontalCenter, 0); // Set Y to 0 for top position
            }
            else 
            {
                this.StartPosition = FormStartPosition.Manual;
                int horizontalCenter = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
                this.Location = new Point(horizontalCenter, 0); // Set Y to 0 for top position
            }
        }

        private void Clavieroverlay_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Clavieroverlay_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
                CenterLabel();
            }
        }

        public void LocalizeControls()
        {
            buttonA.Text = LanguageManager.Instance.GetString("ClavierbtnA");
            buttonB.Text = LanguageManager.Instance.GetString("ClavierbtnB");
            buttonC.Text = LanguageManager.Instance.GetString("ClavierbtnC");
            buttonD.Text = LanguageManager.Instance.GetString("ClavierbtnD");
            buttonE.Text = LanguageManager.Instance.GetString("ClavierbtnE");
            buttonF.Text = LanguageManager.Instance.GetString("ClavierbtnF");
            buttonG.Text = LanguageManager.Instance.GetString("ClavierbtnG");
            buttonH.Text = LanguageManager.Instance.GetString("ClavierbtnH");
            buttonI.Text = LanguageManager.Instance.GetString("ClavierbtnI");
            buttonJ.Text = LanguageManager.Instance.GetString("ClavierbtnJ");        
            buttonK.Text = LanguageManager.Instance.GetString("ClavierbtnK");
            buttonL.Text = LanguageManager.Instance.GetString("ClavierbtnL");
            buttonM.Text = LanguageManager.Instance.GetString("ClavierbtnM");
            buttonN.Text = LanguageManager.Instance.GetString("ClavierbtnN");
            buttonO.Text = LanguageManager.Instance.GetString("ClavierbtnO");
            buttonP.Text = LanguageManager.Instance.GetString("ClavierbtnP");
            buttonQ.Text = LanguageManager.Instance.GetString("ClavierbtnQ");
            buttonR.Text = LanguageManager.Instance.GetString("ClavierbtnR");
            buttonS.Text = LanguageManager.Instance.GetString("ClavierbtnS");
            buttonT.Text = LanguageManager.Instance.GetString("ClavierbtnT");         
            buttonU.Text = LanguageManager.Instance.GetString("ClavierbtnU");
            buttonV.Text = LanguageManager.Instance.GetString("ClavierbtnV");
            buttonW.Text = LanguageManager.Instance.GetString("ClavierbtnW");
            buttonX.Text = LanguageManager.Instance.GetString("ClavierbtnX");
            buttonY.Text = LanguageManager.Instance.GetString("ClavierbtnY");          
            buttonZ.Text = LanguageManager.Instance.GetString("ClavierbtnZ");
            button1.Text = LanguageManager.Instance.GetString("Clavierbtn1");
            button2.Text = LanguageManager.Instance.GetString("Clavierbtn2");
            button3.Text = LanguageManager.Instance.GetString("Clavierbtn3");
            button4.Text = LanguageManager.Instance.GetString("Clavierbtn4");
            button5.Text = LanguageManager.Instance.GetString("Clavierbtn5");
            button6.Text = LanguageManager.Instance.GetString("Clavierbtn6");
            button7.Text = LanguageManager.Instance.GetString("Clavierbtn7");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            label1.ForeColor = colors.Color2;
            rjButton4.BackColor = colors.Color3;
            rjButton2.BackColor = colors.Color3;
            rjButton3.BackColor = colors.Color3;
            rjButton4.ForeColor = colors.Color2;
            rjButton2.ForeColor = colors.Color2;
            rjButton3.ForeColor = colors.Color2;
            roundedPanel2.BackColor = colors.Color3;
        }
    }
}