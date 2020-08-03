using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        string ans = "0";
        public Form1()
        {
            InitializeComponent();
        }

        #region func
        void setText(string text)
        {
            if (screen.Text !=  "0" || isSign(Convert.ToChar(text)))
                screen.Text += text;
            else
                screen.Text = text;
        }

        bool signLast()
        {
            if (screen.Text.Substring(screen.Text.Length - 1) == "+") return true;
            if (screen.Text.Substring(screen.Text.Length - 1) == "-") return true;
            if (screen.Text.Substring(screen.Text.Length - 1) == "*") return true;
            if (screen.Text.Substring(screen.Text.Length - 1) == "/") return true;
            if (screen.Text.Substring(screen.Text.Length - 1) == "^") return true;
            return false;
        }
        bool brOpenLast()
        {
            if (screen.Text.Substring(screen.Text.Length - 1) == "(") return true;
            return false;
        }
        bool brCloseLast()
        {
            if (screen.Text.Substring(screen.Text.Length - 1) == ")") return true;
            return false;
        }

        string getExprInBrackets(string text)
        {
            if (text.Contains("("))
            {
                int level = 0;
                string ans = "";
                for (int i = text.IndexOf('('); i < text.Length; i++)
                {
                    if (text[i] == '(') level++;
                    if (text[i] == ')')
                    {
                        level--;
                        if (level == 0)
                        {
                            ans += text[i];
                            break;
                        }
                    }
                    ans += text[i];
                }
                ans = ans.Substring(1, ans.Length - 1);
                if (ans[ans.Length - 1] == ')') ans = ans.Substring(0, ans.Length - 1);
                return ans;
            }
            return "";
        }

        Queue<string> replaceBrackets(ref string text)
        {
            Queue<string> br = new Queue<string>();
            int count = 0;
            while (text.Contains("("))
            {
                br.Enqueue(getExprInBrackets(text));
                text = text.Replace("(" + getExprInBrackets(text) + ")", "$" + count++);
            }
            return br;
        }

        void setSignTree(char sign, string text, BinaryTree<string> tree)
        {
            Queue<BinaryTree<string>> queue = new Queue<BinaryTree<string>>();
            string[] res = text.Split(sign);

            if(res.Length > 1)
            {
                tree = new BinaryTree<string>(sign.ToString());
                queue.Enqueue(tree);

                for (int i = 1; i < res.Length / 2; i++)
                {
                    tree = queue.First();
                    queue.Dequeue();

                    tree.left = new BinaryTree<string>(sign.ToString());
                    tree.right = new BinaryTree<string>(sign.ToString());
                    queue.Enqueue(tree.left);
                    queue.Enqueue(tree.right);
                }

                for(int i = 0; i < res.Length; i += 2)
                {
                    if (i + 2 > res.Length)
                        queue.First().value = res[i];
                    else
                    {
                        queue.First().left = new BinaryTree<string>(res[i]);
                        queue.First().right = new BinaryTree<string>(res[i + 1]);
                        queue.Dequeue();
                    }
                }
            }
        }

        BinaryTree<string> createTree(string text)
        {
            BinaryTree<string> tree = new BinaryTree<string>("");
            BinaryTree<string> top = tree;
            Queue<string> in_br = replaceBrackets(ref text);
            setSignTree('+', text, tree);
            return top;
        }

        Stack<string> in_br = new Stack<string>();

        void getList(string text, BinaryTree<string> tree)
        {
            if (createTree(text).left != null)
                getList(text, tree.left);

            if (createTree(text).right != null)
                getList(text, tree.right);
        }

        void getResult(string text)
        {
            in_br.Push(text);
            while(getExprInBrackets(text) != "")
            {
                getResult(getExprInBrackets(text));
                text = text.Replace("(" + getExprInBrackets(text) + ")", solve(in_br.Peek()));
                in_br.Pop();
                in_br.Pop();
                in_br.Push(text);
            }
            ans = solve(text);
        }

        string solveForSign(char sign, string text)
        {
            string num1 = "", num2 = "";
            bool is_sign = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (isSignNotMinus(text[i]) && is_sign) break;
                if (text[i] == sign) is_sign = true;

                if (!is_sign) num1 += text[i];
                else num2 += text[i];
                if (isSignNotMinus(text[i]) && text[i] != sign && !is_sign)
                {
                    num1 = "";
                }
            }
            //MessageBox.Show(num1 + "_" + num2.Substring(1));
            return num1 + "_" + num2.Substring(1);
        }

        string solve(string text)
        {
            string num1 = "0", num2 = "0";
            string s;
            while(text.Contains("^"))
            {
                num1 = solveForSign('^', text).Split('_')[0];
                num2 = solveForSign('^', text).Split('_')[1];
                s = num1 + "^" + num2;
                num1 = num1.Replace("$", "-");
                num2 = num2.Replace("$", "-");
                text = text.Replace(s, Math.Pow(Convert.ToDouble(num1), Convert.ToDouble(num2)).ToString());
            }
            while (text.Contains("*"))
            {

                num1 = solveForSign('*', text).Split('_')[0];
                num2 = solveForSign('*', text).Split('_')[1];
                s = num1 + "*" + num2;
                num1 = num1.Replace("$", "-");
                num2 = num2.Replace("$", "-");
                text = text.Replace(s, (Convert.ToDouble(num1) * Convert.ToDouble(num2)).ToString());
            }
            while (text.Contains("/"))
            {
                num1 = solveForSign('/', text).Split('_')[0];
                num2 = solveForSign('/', text).Split('_')[1];
                s = num1 + "/" + num2;
                num1 = num1.Replace("$", "-");
                num2 = num2.Replace("$", "-");
                text = text.Replace(s, (Convert.ToDouble(num1) / Convert.ToDouble(num2)).ToString());
            }
            while (text.Contains("+"))
            {
                num1 = solveForSign('+', text).Split('_')[0];
                num2 = solveForSign('+', text).Split('_')[1];
                s = num1 + "+" + num2;
                num1 = num1.Replace("$", "-");
                num1 = num1.Replace("--", "-");
                num2 = num2.Replace("$", "-");
                num2 = num2.Replace("--", "");
                text = text.Replace(s, (Convert.ToDouble(num1 == "" ? "0" : num1) + Convert.ToDouble(num2)).ToString());
            }
            return text;
        }

        string replaceMinus(string text)
        {
            if (text[0] == '-')
                text = "$" + text.Substring(1);
            text = text.Replace("-", "+$");
            return text;
        }

        void delete()
        {
            if (screen.Text.Length > 1)
                screen.Text = screen.Text.Substring(0, screen.Text.Length - 1);
            else
                screen.Text = "0";
        }

        bool bracketsBalance(string text)
        {
            int level = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '(') level++;
                if (text[i] == ')') level--;
            }
            return level == 0;
        }

        bool isSign(char sign)
        {
            if(sign == '+' || sign == '-' || sign == '*' || sign == '/' || sign == '^') return true;
            return false;
        }

        bool isSignNotMinus(char sign)
        {
            if (sign == '+' || sign == '*' || sign == '/' || sign == '^') return true;
            return false;
        }

        bool isComa(string text)
        {
            bool coma = true;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ',') coma = false;
                if (isSign(text[i])) coma = true;
            }
            return coma;
        }
        #endregion

        #region Act
        private void button_del_Click(object sender, EventArgs e)
        {
            if (screen.Text.Length == 1) screen.Text = "0";
            else delete();
        }

        private void button_eql_Click(object sender, EventArgs e)
        {
            screen.Text = replaceMinus(screen.Text);
            while (!bracketsBalance(screen.Text))
                screen.Text += ")";
            getResult(screen.Text);
            screen.Text = ans;
            if (screen.Text.Length > 10)
            {
                if(screen.Text.Contains(","))
                    screen.Text = screen.Text.Substring(0, 11);
                else
                {

                }
            }
        }
        
        private void screen_Click(object sender, EventArgs e)
        {
            screen.Text = "0";
        }

        #endregion

        #region Signs
        private void button_open_br_Click(object sender, EventArgs e)
        {
            if (signLast() && !brCloseLast() || screen.Text == "0" || brOpenLast())
                setText("(");
        }

        private void button_coma_Click(object sender, EventArgs e)
        {
            if (!signLast() && !brOpenLast() && !brCloseLast() && isComa(screen.Text)) screen.Text += ",";
        }

        private void button_close_br_Click(object sender, EventArgs e)
        {
            if (!signLast() && !bracketsBalance(screen.Text) && !brOpenLast())
                setText(")");
        }

        private void button_pl_Click(object sender, EventArgs e)
        {
            if(signLast())
            {
                delete();
            }
            if (!brOpenLast())
                setText("+");
        }

        private void button_min_Click(object sender, EventArgs e)
        {
            if (signLast())
            {
                delete();
            }
            setText("-");
        }

        private void button_mult_Click(object sender, EventArgs e)
        {
            if (signLast())
            {
                delete();
            }
            if (!brOpenLast())
                setText("*");
        }

        private void button_div_Click(object sender, EventArgs e)
        {
            if (signLast())
            {
                delete();
            }
            if (!brOpenLast())
                setText("/");
        }

        private void button_pow_Click(object sender, EventArgs e)
        {
            if (signLast())
            {
                delete();
            }
            if (!brOpenLast())
                setText("^");
        }
        #endregion

        #region Nums
        private void button1_Click(object sender, EventArgs e)
        {
            setText("1");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            setText("2");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            setText("3");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            setText("4");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            setText("5");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            setText("6");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            setText("7");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            setText("8");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            setText("9");
        }

        private void button0_Click(object sender, EventArgs e)
        {
            if (screen.Text[screen.Text.Length - 1] != '0' || isComa(screen.Text))
                setText("0");
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode > Keys.D0 && e.KeyCode <= Keys.D9) setText(e.KeyCode.ToString().Substring(1));
            if (e.KeyCode == Keys.D0)
                if (screen.Text[screen.Text.Length - 1] != '0' || isComa(screen.Text))
                    setText("0");
            if (e.KeyCode == Keys.Back) delete();
            if(e.KeyCode == Keys.ControlKey)
            {
                screen.Text = replaceMinus(screen.Text);
                while (!bracketsBalance(screen.Text))
                    screen.Text += ")";
                getResult(screen.Text);
                screen.Text = ans;
                if (screen.Text.Length > 10)
                {
                    if (screen.Text.Contains(","))
                        screen.Text = screen.Text.Substring(0, 11);
                    else
                    {

                    }
                }
            }
            if (e.KeyCode == Keys.OemMinus)
            {
                if (signLast())
                {
                    delete();
                }
                setText("-");
            }
            if(e.KeyCode == Keys.Oemplus)
            {
                if (signLast())
                {
                    delete();
                }
                if (!brOpenLast())
                    setText("+");
            }
            if(e.KeyCode == Keys.OemQuestion)
            {
                if (signLast())
                {
                    delete();
                }
                if (!brOpenLast())
                    setText("/");
            }
            if (e.KeyCode == Keys.X)
            {
                if (signLast())
                {
                    delete();
                }
                if (!brOpenLast())
                    setText("*");
            }
            if (e.KeyCode == Keys.OemOpenBrackets)
            {
                if (signLast() && !brCloseLast() || screen.Text == "0" || brOpenLast())
                    setText("(");
            }
        }
    }
}
