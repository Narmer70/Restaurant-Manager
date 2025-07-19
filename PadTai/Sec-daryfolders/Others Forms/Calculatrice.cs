using System;
using System.Data;
using System.Drawing;
using PadTai.Classes;
using System.Windows.Forms;
using PadTai.Fastcheckfiles;
using PadTai.Classes.Others;
using System.Text.RegularExpressions;


namespace PadTai
{
    public partial class Calculatrice : UserControl
    {
        private FontResizer fontResizer;
        private ControlResizer resizer;

        private string currentBtnIndex = "";
        private string currentCommand = "";
        Calculator Calc = new Calculator();
        string evaluationString = "";
        bool isSecondPage = false;
        int countOfBracket = 0;
        bool isDegree = false;
        Fastcheck check;


        public Calculatrice(Fastcheck check)
        {
            InitializeComponent();
            InitializeControlResizer();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
            this.check = check;

            lblCalculatorMode.Visible = false;
            closeButton.Visible = false;
            LocalizeControls();
            ApplyTheme();
        }

        #region Resise
        private void InitializeControlResizer()
        {
            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(btn00);
            resizer.RegisterControl(btn0);
            resizer.RegisterControl(btn01);
            resizer.RegisterControl(btn02);
            resizer.RegisterControl(btn1);
            resizer.RegisterControl(btn10);
            resizer.RegisterControl(btn11);
            resizer.RegisterControl(btn12);
            resizer.RegisterControl(btn2);
            resizer.RegisterControl(btn3);
            resizer.RegisterControl(btn4);
            resizer.RegisterControl(btn5);
            resizer.RegisterControl(btn6);
            resizer.RegisterControl(btn7);
            resizer.RegisterControl(btn8);
            resizer.RegisterControl(btn9);
            resizer.RegisterControl(btn20);
            resizer.RegisterControl(btn21);
            resizer.RegisterControl(btn22);
            resizer.RegisterControl(btn30);
            resizer.RegisterControl(btn31);
            resizer.RegisterControl(btn32);
            resizer.RegisterControl(btn40);
            resizer.RegisterControl(btn41);
            resizer.RegisterControl(btn42);
            resizer.RegisterControl(panel1);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(rjButton3);
            resizer.RegisterControl(rjButton4);
            resizer.RegisterControl(rjButton5);
            resizer.RegisterControl(rjButton6);
            resizer.RegisterControl(rjButton7);
            resizer.RegisterControl(rjButton8);
            resizer.RegisterControl(rjButton9);
            resizer.RegisterControl(rjButton10);
            resizer.RegisterControl(rjButton11);
            resizer.RegisterControl(rjButton12);
            resizer.RegisterControl(btnClear);
            resizer.RegisterControl(btnEquals);
            resizer.RegisterControl(btnBrackets);
            resizer.RegisterControl(btnDivide);
            resizer.RegisterControl(btnChangeSign);
            resizer.RegisterControl(btnBackSpace);
            resizer.RegisterControl(btnModuloDivison);
            resizer.RegisterControl(btnMultiplication);
            resizer.RegisterControl(btnShowSideBar);
            resizer.RegisterControl(btnSubtraction);
            resizer.RegisterControl(btnAddition);
            resizer.RegisterControl(btnDecimal);
            resizer.RegisterControl(btndatesub);
            resizer.RegisterControl(sideBar);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(button3);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(label6);
            resizer.RegisterControl(textBox2);
            resizer.RegisterControl(btnp1Return);
            resizer.RegisterControl(closeButton);
            resizer.RegisterControl(txtBoxFirst);
            resizer.RegisterControl(txtBoxResult);
            resizer.RegisterControl(txtBoxResult);
            resizer.RegisterControl(txtBoxSecond);
            resizer.RegisterControl(radioButton1);
            resizer.RegisterControl(radioButton2);
            resizer.RegisterControl(radioButton3);
            resizer.RegisterControl(radioButton4);
            resizer.RegisterControl(radioButton5);
            resizer.RegisterControl(radioButton6);
            resizer.RegisterControl(radioButton7);
            resizer.RegisterControl(radioButton8);
            resizer.RegisterControl(radioButton9);
            resizer.RegisterControl(radioButton10);
            resizer.RegisterControl(btnbacktoslide);
            resizer.RegisterControl(dateTimePicker1);
            resizer.RegisterControl(dateTimePicker2);            
            resizer.RegisterControl(txtBoxExpression);
            resizer.RegisterControl(radioButtonFirst);
            resizer.RegisterControl(radioButtonSecond);
            resizer.RegisterControl(lblCalculatorMode);
            resizer.RegisterControl(btnTemperatureConverter);
            resizer.RegisterControl(temperatureConversionBanner);
        }
        #endregion

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
        }

        private void Calculatrice_Load(object sender, EventArgs e)
        {
            txtBoxExpression.Select();       
        }


        private void HandleClick(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            UpdateExpression(b.Text);
            txtBoxExpression.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            txtBoxResult.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            txtBoxResult.ForeColor = Color.Gray;
            txtBoxExpression.ForeColor = Color.Gray;
        }


        private void UpdateExpression(string ex)
        {
            evaluationString += ex;
            txtBoxExpression.Text += ex;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DishGroupControl dishGroupControl = new DishGroupControl(check, null);
            check.AddUserControl(dishGroupControl);
        }

        private void Eval()
        {
            try
            {
                if (currentCommand != "")
                    Parse();
            }
            catch (Exception)
            {
            }

            DataTable dt = new DataTable();
            try
            {
                double result = Convert.ToDouble(dt.Compute(evaluationString, String.Empty));
                txtBoxResult.Text = Math.Round(result, 5).ToString();
            }
            catch (Exception)
            {
                txtBoxResult.Text = "";
            }

        }

        private void Parse()
        {
            //Declaration of variable
            int startPosition, endPosition, index;

            string parsedNumber = "";

            double number, result;

            //ScreenNumber is 1 if current screen is first screen otherwise 2
            int screenNumber = isSecondPage ? 2 : 1;

            //getting status
            int status = int.Parse(String.Concat(screenNumber.ToString(), currentBtnIndex));

            switch (status)
            {

                case 102://Square Root
                    {
                        startPosition = evaluationString.IndexOf("sqrt(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where sqrt( length is 5 so
                        for (index = (startPosition + 5); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.SquareRoot(number), 2);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;
                    }
                case 110://sin
                    {
                        startPosition = evaluationString.IndexOf("sin(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where sin( length is 4 so
                        for (index = (startPosition + 4); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Check calculator is in degree mode or not
                        if (isDegree) { number = Calc.DegreeToRadian(number); }
                        //Round result
                        result = Math.Round(Calc.Sin(number), 2);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 111://cos
                    {
                        startPosition = evaluationString.IndexOf("cos(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where cos( length is 4 so
                        for (index = (startPosition + 4); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Check calculator is in degree mode or not
                        if (isDegree) { number = Calc.DegreeToRadian(number); }

                        //Round result
                        result = Math.Round(Calc.Cos(number), 2);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 112://tan
                    {
                        startPosition = evaluationString.IndexOf("tan(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where tan( length is 4 so
                        for (index = (startPosition + 4); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Check calculator is in degree mode or not
                        if (isDegree) { number = Calc.DegreeToRadian(number); }

                        //Round result
                        result = Math.Round(Calc.Tan(number), 2);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 120://ln
                    {
                        startPosition = evaluationString.IndexOf("ln(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where ln( length is 3 so
                        for (index = (startPosition + 3); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.Ln(number), 4);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 121://log
                    {
                        startPosition = evaluationString.IndexOf("log(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where log( length is 4 so
                        for (index = (startPosition + 4); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.Log(number), 4);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 130://e^
                    {
                        startPosition = evaluationString.IndexOf("e^(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where e^( length is 3 so
                        for (index = (startPosition + 3); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.Exponential(number), 5);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 140://absolute
                    {
                        startPosition = evaluationString.IndexOf("abs(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where abs( length is 4 so
                        for (index = (startPosition + 4); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Calc.Absolute(number);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 202://Cube root
                    {
                        startPosition = evaluationString.IndexOf("cbrt(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where cbrt( length is 5 so
                        for (index = (startPosition + 5); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.CubeRoot(number), 2);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 210://SineInverse
                    {
                        startPosition = evaluationString.IndexOf("Asin(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where Asin( length is 5 so
                        for (index = (startPosition + 5); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.SineInverse(number), 2);
                        //Check calculator is in degree mode or not
                        if (isDegree) { result = Math.Round(Calc.RadianToDegree(result), 3); }

                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;
                    }
                case 211://CosInverse
                    {
                        startPosition = evaluationString.IndexOf("Acos(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where Acos( length is 5 so
                        for (index = (startPosition + 5); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.CosInverse(number), 2);
                        //Check calculator is in degree mode or not
                        if (isDegree) { result = Math.Round(Calc.RadianToDegree(result), 3); }
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 212://TanInverse
                    {
                        startPosition = evaluationString.IndexOf("Atan(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where Atan( length is 5 so
                        for (index = (startPosition + 5); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.TanInverse(number), 2);
                        //Check calculator is in degree mode or not
                        if (isDegree) { result = Math.Round(Calc.RadianToDegree(result), 3); }
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 220://Sine Hyperbolic
                    {
                        startPosition = evaluationString.IndexOf("sinh(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where sinh( length is 5 so
                        for (index = (startPosition + 5); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.SineHyperbolic(number), 2);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 221://Cos Hyperbolic
                    {
                        startPosition = evaluationString.IndexOf("cosh(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where cosh( length is 5 so
                        for (index = (startPosition + 5); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.CosHyperbolic(number), 2);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 222://Tan Hyperbolic
                    {
                        startPosition = evaluationString.IndexOf("tanh(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where tanh( length is 5 so
                        for (index = (startPosition + 5); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.TanHyperbolic(number), 2);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 230://Inverse Sine Hyperbolic
                    {
                        startPosition = evaluationString.IndexOf("Asinh(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where Asinh( length is 6 so
                        for (index = (startPosition + 6); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.InverseSineHyperbolic(number), 2);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 231://Inverse Cos Hyperbolic
                    {
                        startPosition = evaluationString.IndexOf("Acosh(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where Acosh( length is 6 so
                        for (index = (startPosition + 6); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.InverseCosHyperbolic(number), 2);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 232://Inverse Tan Hyperbolic
                    {
                        startPosition = evaluationString.IndexOf("Atanh(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where Atanh( length is 6 so
                        for (index = (startPosition + 6); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.InverseTanHyperbolic(number), 2);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                case 240://2^
                    {
                        startPosition = evaluationString.IndexOf("2^(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where 2^( length is 3 so
                        for (index = (startPosition + 3); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = double.Parse(parsedNumber);
                        //Round result
                        result = Math.Round(Calc.PowerOf2(number), 2);
                        //Than replaces in evaluationString
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                //Same process on three cases so
                case 131:
                case 132:
                case 241:
                    {
                        string givenNumber = "";
                        startPosition = evaluationString.IndexOf("^(");
                        endPosition = evaluationString.IndexOf(")", startPosition);
                        //Where ^( length is 2 so
                        for (index = (startPosition + 2); index < endPosition; index++)
                        {
                            //Getting Number Lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        double power = double.Parse(parsedNumber);
                        //To get the actual number
                        index = startPosition - 1;

                        //Replace this with regular expression for short hand
                        while (index >= 0 && (evaluationString[index] != '+' && txtBoxExpression.Text[index] != '-' && txtBoxExpression.Text[index] != '*' && txtBoxExpression.Text[index] != '/' && txtBoxExpression.Text[index] != '%' && txtBoxExpression.Text[index] != '(' && txtBoxExpression.Text[index] != ')'))
                        {
                            givenNumber += evaluationString[index];
                            index--;
                        }
                        //Result
                        result = Math.Round(Calc.Power(double.Parse(givenNumber), power), 2);
                        //Replace result on txtBoxExpression
                        evaluationString = evaluationString.Remove(index + 1, endPosition - index).Insert(index + 1, result.ToString());

                        break;
                    }
                case 242://Factorial
                    {
                        startPosition = evaluationString.IndexOf("facto(");
                        endPosition = evaluationString.IndexOf(')', startPosition);
                        //Where ( length is 6 so
                        for (index = (startPosition + 6); index < endPosition; index++)
                        {
                            //Getting Number lies inside Bracket
                            parsedNumber += evaluationString[index];
                        }
                        //Parsing into float
                        number = Math.Abs(double.Parse(parsedNumber));
                        //Round result
                        result = Calc.Factorial(number);
                        //Than replaces in txBoxExpression
                        evaluationString = evaluationString.Remove(startPosition, (endPosition - startPosition + 1)).Insert(startPosition, result.ToString());
                        break;

                    }
                //On Default Case do nothing 

                default: { break; }

            }
        }

        private void btnEquals_Click(object sender, EventArgs e)
        {
            txtBoxExpression.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            txtBoxResult.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            txtBoxResult.ForeColor = this.ForeColor;
            txtBoxExpression.ForeColor = Color.Gray;
        }

        private void SwapValues()
        {
            evaluationString = txtBoxResult.Text;
            txtBoxExpression.Text = txtBoxResult.Text;
            txtBoxResult.Text = "";
            countOfBracket = 0;
            txtBoxExpression.Select(txtBoxExpression.Text.Length, 0);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtBoxExpression.Text = "";
            txtBoxResult.Text = "";
            countOfBracket = 0;
            evaluationString = "";
            txtBoxExpression.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            txtBoxResult.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            txtBoxResult.ForeColor = Color.Gray;
            txtBoxExpression.ForeColor = Color.Gray;
        }

        private void btnBackSpace_Click(object sender, EventArgs e)
        {
            BackSpaceClick();
        }

        private void BackSpaceClick()
        {
            if (txtBoxExpression.Text.Length > 0)
            {
                //Temprorarily Hold Expression
                string tempExpression = txtBoxExpression.Text;

                //If removed char is ) then
                if (tempExpression[tempExpression.Length - 1] == '(')
                {
                    countOfBracket = 0;
                }
                //If removed char is ) then
                if (tempExpression[tempExpression.Length - 1] == ')')
                {
                    countOfBracket++;
                }
                txtBoxExpression.Text = tempExpression.Remove(tempExpression.Length - 1);
                //Set updated value in evaluationString as well
                evaluationString = txtBoxExpression.Text;
                //Clear ResultTextBox
                txtBoxResult.Text = "";
            }
            txtBoxExpression.Select(txtBoxExpression.Text.Length, 0);
            //Then evaluate again
            Eval();
            txtBoxExpression.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            txtBoxResult.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            txtBoxResult.ForeColor = Color.Gray;
        }

        private void txtBoxExpression_TextChanged(object sender, EventArgs e)
        {
            Eval();

            if (txtBoxExpression.Text.Split('.').Length - 1 > 1)
            {
                txtBoxExpression.Text = txtBoxExpression.Text.Remove(txtBoxExpression.Text.Length - 1);
                txtBoxExpression.SelectionStart = txtBoxExpression.Text.Length;
            }
        }

        private void btnBrackets_Click(object sender, EventArgs e)
        {
            //Regular Expression to check if string ends with any algebric opertaor
            Regex regex = new Regex(@"[+\-% */]$");
            //Regular expression to check if strikng ends with numeric digit
            Regex numericEnd = new Regex(@"\d$");

            //if Expression is empty
            if (countOfBracket == 0 && txtBoxExpression.Text.Length == 0)
            {
                UpdateExpression("(");
                countOfBracket++;
            }

            //If opening bracket is on last index
            else if (txtBoxExpression.Text.LastIndexOf('(') == txtBoxExpression.Text.Length - 1)
            {
                UpdateExpression("(");
                countOfBracket++;
            }
            //If string ends with algebric operator
            else if (regex.IsMatch(txtBoxExpression.Text))
            {
                UpdateExpression("(");
                countOfBracket++;
            }
            //
            else if (countOfBracket == 0 && numericEnd.IsMatch(txtBoxExpression.Text))
            {
                UpdateExpression("*(");
                countOfBracket++;

            }
            //Otherwise push closing bracket
            else
            {
                UpdateExpression(")");
                countOfBracket--;
            }

            Eval();// Call Eval after modifying expression
        }

        private void btnChangeSign_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex(@"[+\-%*/(]$");
            if (txtBoxExpression.Text.Length == 0)
            {
                UpdateExpression("(-");
                countOfBracket++;
            }
            else if (regex.IsMatch(txtBoxExpression.Text))

            {
                UpdateExpression("(-");
                countOfBracket++;


            }
            else if (txtBoxExpression.Text.EndsWith(")"))
            {
                UpdateExpression("*(-");
                countOfBracket++;
            }
        }

        private void ParseFunctionText(string Text, string name)
        {
            Regex regex = new Regex(@"[+\-% */]$");


            //Declaration and Initialization of variable
            string templateString = "", command = Text;
            //Checking command Text to perform operation
            if (Text == "√") { command = "sqrt"; }
            if (Text == "3√x") { command = "cbrt"; }
            if (Text == "e^x") { command = "e^"; }
            if (Text == "|x|") { command = "abs"; }
            if (Text == "2^x") { command = "2^"; }
            if (Text == "x!") { command = "facto"; }

            //To save the status of currently which button is clicked
            currentCommand = command;
            templateString = name;

            //To get Last Two char from templateString
            currentBtnIndex = templateString.Substring(templateString.Length - 2);

            if (txtBoxExpression.Text.Length == 0 || regex.IsMatch(txtBoxExpression.Text))
            {
                //Updating Expression
                UpdateExpression(command + "(");
                //Increment countOfBracket
                countOfBracket++;
            }

            else
            {
                //Updating Expression
                UpdateExpression("*" + command + "(");
                //Increment countOfBracket
                countOfBracket++;
            }

        }

        private void btn00_Click(object sender, EventArgs e)
        {
            if (!isSecondPage)
            {
                btn00.Text = ">";
                btn10.Text = "Asin";
                btn20.Text = "sinh";
                btn30.Text = "Asinh";
                btn40.Text = "2^x";
                btn11.Text = "Acos";
                btn21.Text = "cosh";
                btn31.Text = "Acosh";
                btn41.Text = "x^3";
                btn02.Text = "3√x";
                btn12.Text = "Atan";
                btn22.Text = "tanh";
                btn32.Text = "Atanh";
                btn42.Text = "x!";
                isSecondPage = true;
                btn01.Enabled = false;
            }
            else
            {
                btn00.Text = "<";
                btn10.Text = "Sin";
                btn20.Text = "ln";
                btn30.Text = "e^x";
                btn40.Text = "|x|";
                btn11.Text = "Cos";
                btn21.Text = "log";
                btn31.Text = "x^2";
                btn41.Text = "π";
                btn02.Text = "√";
                btn12.Text = "tan";
                btn22.Text = "1/x";
                btn32.Text = "x^y";
                btn42.Text = "e";
                isSecondPage = false;
                btn01.Enabled = true;
            }
        }

        private void txtBoxExpression_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SwapValues();
            }
            else if (e.KeyChar == (char)Keys.Back)
            {
                BackSpaceClick();
                e.Handled = true;
            }
            else if (e.KeyChar == 37 || e.KeyChar == 40 || e.KeyChar == 41 || e.KeyChar == 42 || e.KeyChar == 43 || e.KeyChar == 45 || e.KeyChar == 46 || e.KeyChar == 47)
            {
                e.Handled = false;
                evaluationString += ((char)e.KeyChar).ToString();
            }
            else
            {
                bool status = !char.IsDigit(e.KeyChar);
                if (status) { e.Handled = true; }
                else
                {
                    evaluationString += ((char)e.KeyChar).ToString();
                    e.Handled = false;
                }

            }
        }

        private void btn01_Click(object sender, EventArgs e)
        {
            if (!isDegree)
            {
                btn01.Text = "Rad";
                lblCalculatorMode.Text = "Deg";
                isDegree = true;
            }
            else
            {
                btn01.Text = "Deg";
                lblCalculatorMode.Text = "Rad";
                isDegree = false;
            }
        }

        private void btn41_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex(@"[+\-% */]$");

            if (!isSecondPage)
            {
                if (txtBoxExpression.Text.Length == 0 || regex.IsMatch(txtBoxExpression.Text))
                {
                    UpdateExpression(Calc.PI().ToString());
                }
                else
                {
                    UpdateExpression("*" + Calc.PI().ToString());

                }
            }
            //On second page
            else
            {
                Button b = (Button)sender;
                ParsePowerFunctionText(b.Text, b.Name);
            }
        }

        private void ParsePowerFunctionText(string command, string Name)
        {
            Regex regex = new Regex(@"[+\-% */()]$");

            string expression = txtBoxExpression.Text;
            //To Save the status of currently which button is clicked
            currentBtnIndex = Name.Substring(Name.Length - 2);
            currentCommand = command;

            //Check Whether txtBoxExpression is empty or not and it doesn't ends with /,*,-,+,%,(,)
            if (!(expression.Length == 0 || regex.IsMatch(txtBoxExpression.Text)))
            {
                if (command == "x^2")
                {
                    UpdateExpression("^(2)");
                }
                else if (command == "x^3")
                {

                    UpdateExpression("^(3)");
                }
                else
                {
                    UpdateExpression("^(");
                    countOfBracket++;
                }
            }

        }

        private void btn42_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex(@"[+\-% */]$");

            if (!isSecondPage)
            {

                if (txtBoxExpression.Text.Length == 0 || regex.IsMatch(txtBoxExpression.Text))
                {
                    UpdateExpression(Calc.E().ToString());
                }
                //On second page
                else
                {
                    UpdateExpression("*" + Calc.E().ToString());

                }
            }
            //On second page
            else
            {
                Button b = (Button)sender;
                ParseFunctionText(b.Text, b.Name);
            }
        }

        private void btn22_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex(@"[+\-% */]$");

            if (!isSecondPage)//I.e When (1/x)button is clicked
            {
                if (txtBoxExpression.Text.Length == 0 || regex.IsMatch(txtBoxExpression.Text))
                {
                    UpdateExpression("1/");
                }
                else
                {
                    UpdateExpression("*1/");
                }


            }
            //On Second Page
            else
            {
                Button b = (Button)sender;
                ParseFunctionText(b.Text, b.Name);
            }
        }

        private void PowerFunction_Clicked(object sender, EventArgs e)
        {
            if (!isSecondPage)
            {
                Button b = (Button)sender;
                ParsePowerFunctionText(b.Text, b.Name);
            }
            else
            {
                Button b = (Button)sender;
                ParseFunctionText(b.Text, b.Name);
            }
        }

        private void btnShowSideBar_Click(object sender, EventArgs e)
        {
            if (sideBar.Visible == false)
            {
                sideBar.Visible = true;
            }
            else
            {
                sideBar.Visible = false;
            }
        }

        private void btnFunction_Clicked(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            ParseFunctionText(b.Text, b.Name);
            txtBoxExpression.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            txtBoxResult.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            txtBoxResult.ForeColor = Color.Gray;
        }

        private void txtBoxFirst_TextChanged(object sender, EventArgs e)
        {
            converttemperatures();
            ActiveControl = (Control)sender;

            if (txtBoxFirst.Text.Split(',').Length - 1 > 1)
            {
                txtBoxFirst.Text = txtBoxFirst.Text.Remove(txtBoxFirst.Text.Length - 1);
                txtBoxFirst.SelectionStart = txtBoxFirst.Text.Length;
            }
        }

        private void converttemperatures() 
        {
            if (radioButtonFirst.Checked)
            {
                try
                {
                    string celciusInp = txtBoxFirst.Text;
                    if (celciusInp == "")
                    {
                        txtBoxSecond.Text = "";
                    }
                    else
                    {
                        double celciusInput = double.Parse(celciusInp);
                        txtBoxSecond.Text = $"{Math.Round(((celciusInput * 9) / 5 + 32), 2).ToString()}°F";

                    }

                }
                catch (Exception)
                {

                }
            }
            else if (radioButtonSecond.Checked)
            {
                try
                {
                    string fahrenheitInp = txtBoxFirst.Text;
                    if (fahrenheitInp == "")
                    {
                        txtBoxSecond.Text = "";
                    }
                    else
                    {
                        double fahrenheitInput = double.Parse(fahrenheitInp);
                        txtBoxSecond.Text = $"{Math.Round(((fahrenheitInput - 32) * (double)5 / 9), 2).ToString()}°C";

                    }

                }
                catch (Exception)
                {

                }
            }
            else if (radioButton1.Checked)
            {
                try
                {
                    string celsiusInp = txtBoxFirst.Text;
                    if (celsiusInp == "")
                    {
                        txtBoxSecond.Text = "";
                    }
                    else
                    {
                        double celsiusInput = double.Parse(celsiusInp);
                        txtBoxSecond.Text = $"{Math.Round((celsiusInput + 273.15), 2).ToString()}°K";
                    }
                }
                catch (Exception)
                {
                    // Handle exception
                }

            }
            else if (radioButton4.Checked)
            {
                try
                {
                    string fahrenheitInp = txtBoxFirst.Text;
                    if (fahrenheitInp == "")
                    {
                        txtBoxSecond.Text = "";
                    }
                    else
                    {
                        double fahrenheitInput = double.Parse(fahrenheitInp);
                        double celsius = (fahrenheitInput - 32) * (double)5 / 9;
                        txtBoxSecond.Text = $"{Math.Round((celsius + 273.15), 2).ToString()}°K";
                    }
                }
                catch (Exception)
                {
                    // Handle exception
                }
            }
            else if (radioButton2.Checked)
            {
                try
                {
                    string kelvinInp = txtBoxFirst.Text;
                    if (kelvinInp == "")
                    {
                        txtBoxSecond.Text = "";
                    }
                    else
                    {
                        double kelvinInput = double.Parse(kelvinInp);
                        txtBoxSecond.Text = $"{Math.Round((kelvinInput - 273.15), 2).ToString()}°C";
                    }
                }
                catch (Exception)
                {
                    // Handle exception
                }
            }
            else if (radioButton3.Checked)
            {
                try
                {
                    string kelvinInp = txtBoxFirst.Text;
                    if (kelvinInp == "")
                    {
                        txtBoxSecond.Text = "";
                    }
                    else
                    {
                        double kelvinInput = double.Parse(kelvinInp);
                        double celsius = kelvinInput - 273.15;
                        txtBoxSecond.Text = $"{Math.Round(((celsius * 9) / 5 + 32), 2).ToString()}°F";
                    }
                }
                catch (Exception)
                {
                    // Handle exception
                }
            }
        }

        private void btnTConverter_Click(object sender, EventArgs e)
        {
            sideBar.Visible = false;
            temperatureConversionBanner.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sideBar.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            temperatureConversionBanner.Visible = false;
        }

        private void radioButtonFirst_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonFirst.Checked)
            {
                label3.Text = "Celsius:";
                label4.Text = "Fahrenheit:";
            }            
            converttemperatures();
        }


        private void radioButtonSecond_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSecond.Checked)
            {
                label3.Text = "Fahrenheit:";
                label4.Text = "Celsius:";
            }
            converttemperatures();
        }

        private void btnbacktoslide_Click(object sender, EventArgs e)
        {
            temperatureConversionBanner.Visible = false;
            sideBar.Visible = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                label3.Text = "Celsius:";
                label4.Text = "Kelvin:";
            }
            converttemperatures();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                label3.Text = "Kelvin:";
                label4.Text = "Celsius:";
            }
            converttemperatures();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                label3.Text = "Kelvin:";
                label4.Text = "Fahrenheit:";
            }
            converttemperatures();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                label3.Text = "Fahrenheit:";
                label4.Text = "Kelvin:";
            }
            converttemperatures();
        }

        private void btndatesub_Click(object sender, EventArgs e)
        {
            sideBar.Visible = false;
            panel1.Visible = true;
        }

        private void btnp1Return_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            sideBar.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            CalculateDateDifference();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            CalculateDateDifference();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            CalculateDateDifference();
        }      
        
        private void CalculateDateDifference()
        {
            DateTime date1 = dateTimePicker1.Value;
            DateTime date2 = dateTimePicker2.Value;

            TimeSpan difference = date1 - date2; 

            if (radioButton6.Checked)
            {
                label5.Text = "Minutes";
                textBox2.Text = $"{difference.TotalMinutes:F2}";
            }
            else if (radioButton5.Checked)
            {
                label5.Text = "Hours";
                textBox2.Text = $"{difference.TotalHours:F2}";
            }
            else if (radioButton7.Checked)
            {
                label5.Text = "Days";
                textBox2.Text = $"{difference.TotalDays:F2}";
            }
            else if (radioButton10.Checked)
            {
                label5.Text = "Years";
                textBox2.Text = $"{difference.TotalDays / 365.0:F3}";
            }
            else if (radioButton8.Checked)
            {
                label5.Text = "Weeks";
                textBox2.Text = $"{difference.TotalDays / 7:F2}";
            }
            else if (radioButton9.Checked)
            {
                label5.Text = "Seconds";
                textBox2.Text = $"{difference.TotalSeconds:F2}";
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            ActiveControl.Focus();
            Button button = (Button)sender;
            txtBoxFirst.Text = txtBoxFirst.Text + button.Text;
        }

        private void rjButton12_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBoxFirst.Text))
            {
                {
                    txtBoxFirst.Text = txtBoxFirst.Text.Substring(0,(txtBoxFirst.Text.Length - 1));
                }
            }
            else
            {
                return;
            }
        }


        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            label3.ForeColor = this.ForeColor;
            label4.ForeColor = this.ForeColor;
            label5.ForeColor = this.ForeColor;
            textBox2.ForeColor = this.ForeColor;
            txtBoxFirst.ForeColor = this.ForeColor;
            btnp1Return.BackColor = this.BackColor;
            btnp1Return.ForeColor = this.ForeColor;
            txtBoxResult.ForeColor = this.ForeColor;
            txtBoxResult.BackColor = this.BackColor;
            txtBoxSecond.ForeColor = this.ForeColor;
            rjButton1.BackColor = colors.Color3;
            rjButton2.BackColor = colors.Color3;
            rjButton3.BackColor = colors.Color3;
            rjButton4.BackColor = colors.Color3;
            rjButton5.BackColor = colors.Color3;
            rjButton6.BackColor = colors.Color3;
            rjButton7.BackColor = colors.Color3;
            rjButton8.BackColor = colors.Color3;
            rjButton9.BackColor = colors.Color3;
            rjButton10.BackColor = colors.Color3;
            rjButton11.BackColor = colors.Color3;
            rjButton12.BackColor = colors.Color3;
            btnbacktoslide.ForeColor = this.ForeColor;
            btnbacktoslide.BackColor = this.BackColor;
            dateTimePicker1.TextColor = this.ForeColor;
            dateTimePicker2.TextColor = this.ForeColor;
            txtBoxExpression.BackColor = this.BackColor;
            txtBoxExpression.ForeColor = this.ForeColor;
            lblCalculatorMode.ForeColor = this.ForeColor;
            textBox2.BackColorRounded = colors.Color3;
            txtBoxFirst.BackColorRounded = colors.Color3;
            txtBoxSecond.BackColorRounded = colors.Color3;
            dateTimePicker1.BackColorCustom = colors.Color3;
            dateTimePicker2.BackColorCustom = colors.Color3;
            dateTimePicker1.CalendarIcon = colors.Image2;
            dateTimePicker2.CalendarIcon = colors.Image2;


            panel1.BackColor = this.BackColor;
            button1.ForeColor = this.ForeColor;
            button2.ForeColor = this.ForeColor;
            button3.ForeColor = this.ForeColor;
            button1.BackColor = this.BackColor;
            button2.BackColor = this.BackColor;
            button3.BackColor = this.BackColor;
            rjButton1.ForeColor = this.ForeColor;
            rjButton2.ForeColor = this.ForeColor;
            rjButton3.ForeColor = this.ForeColor;
            rjButton4.ForeColor = this.ForeColor;
            rjButton5.ForeColor = this.ForeColor;
            rjButton6.ForeColor = this.ForeColor;
            rjButton7.ForeColor = this.ForeColor;
            rjButton8.ForeColor = this.ForeColor;
            rjButton9.ForeColor = this.ForeColor;
            rjButton10.ForeColor = this.ForeColor;
            rjButton11.ForeColor = this.ForeColor;
            rjButton12.ForeColor = this.ForeColor;
            radioButton1.ForeColor = this.ForeColor;
            radioButton2.ForeColor = this.ForeColor;
            radioButton3.ForeColor = this.ForeColor;
            radioButton4.ForeColor = this.ForeColor;
            radioButton5.ForeColor = this.ForeColor;
            radioButton6.ForeColor = this.ForeColor;
            radioButton7.ForeColor = this.ForeColor;
            radioButton8.ForeColor = this.ForeColor;
            radioButton9.ForeColor = this.ForeColor;
            radioButton10.ForeColor = this.ForeColor;
            radioButtonFirst.ForeColor = this.ForeColor;    
            radioButtonSecond.ForeColor = this.ForeColor;
            temperatureConversionBanner.BackColor = this.BackColor;
            //button1.FlatAppearance.MouseOverBackColor = this.BackColor;
            //button2.FlatAppearance.MouseOverBackColor = this.BackColor;
            //button3.FlatAppearance.MouseOverBackColor = this.BackColor;
            //button1.FlatAppearance.MouseDownBackColor = this.BackColor;
            //button2.FlatAppearance.MouseDownBackColor = this.BackColor;
            //button3.FlatAppearance.MouseDownBackColor = this.BackColor;
            //btnp1Return.FlatAppearance.MouseOverBackColor = this.BackColor;
            //btnp1Return.FlatAppearance.MouseDownBackColor = this.BackColor;
            //btnbacktoslide.FlatAppearance.MouseOverBackColor = this.BackColor;
            //btnbacktoslide.FlatAppearance.MouseDownBackColor = this.BackColor;
        }
    }




    public class Calculator
    {
        // Cube Root
        public double CubeRoot(double num) => Math.Pow(num, 1.0 / 3.0);

        // Square Root
        public double SquareRoot(double num) => Math.Pow(num, 0.5);

        // Tan Inverse
        public double TanInverse(double num) => Math.Atan(num);

        // Sine Inverse
        public double SineInverse(double num) => Math.Asin(num);

        // Cos Inverse
        public double CosInverse(double num)
        {
            if (Math.Abs(num) > 1) throw new ArgumentException("Num");
            return Math.Acos(num);
        }

        // Tan Hyperbolic
        public double TanHyperbolic(double num) => Math.Tanh(num);

        // Cos Hyperbolic
        public double CosHyperbolic(double num) => Math.Cosh(num);

        // Sine Hyperbolic
        public double SineHyperbolic(double num) => Math.Sinh(num);

        // Inverse Tan Hyperbolic
        public double InverseTanHyperbolic(double num)
        {
            if (Math.Abs(num) > 1) throw new ArgumentException("Num");
            return 0.5 * Math.Log((1 + num) / (1 - num));
        }

        // Inverse Cos Hyperbolic
        public double InverseCosHyperbolic(double num) => Math.Log(num + Math.Sqrt(num * num - 1));

        // Inverse Sine Hyperbolic
        public double InverseSineHyperbolic(double num) => Math.Log(num + Math.Sqrt(num * num + 1));

        // Tan
        public double Tan(double num) => Math.Tan(num);

        // Cos
        public double Cos(double num) => Math.Cos(num);

        // Sin
        public double Sin(double num) => Math.Sin(num);

        // Ln
        public double Ln(double num) => Math.Log(num);

        // Log
        public double Log(double num) => Math.Log10(num);

        // Exponential
        public double Exponential(double num) => Math.Pow(Math.E, num);

        // Absolute
        public double Absolute(double num) => Math.Abs(num);

        // PI
        public double PI() => Math.PI;

        // E
        public double E() => Math.E;

        // Power of 2
        public double PowerOf2(double num) => Math.Pow(2, num);

        // Power
        public double Power(double num, double power) => Math.Pow(num, power);

        // Radian to Degree conversion
        public double DegreeToRadian(double num) => num * (Math.PI / 180);

        // Degree to Radian conversion
        public double RadianToDegree(double num) => num * (180 / Math.PI);

        // Factorial
        public int Factorial(double num)
        {
            int factorial = 1;
            for (int index = 1; index <= num; index++)
            {
                factorial *= index;
            }
            return factorial;
        }
    }
}
