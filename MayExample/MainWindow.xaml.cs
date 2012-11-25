using System;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Strilanc.Value;

namespace MayExample {
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();

            // see ExampleMayUtilityMethods.cs for implementations of MayParseX, MaySqrt, MayAll, etc

            Show("A as BigInteger", (a, b) => 
                a.MayParseBigInteger());
            
            Show("B as BigInteger", (a, b) => 
                b.MayParseBigInteger());
            
            Show("A as Int32", (a, b) => 
                a.MayParseInt32());
            
            Show("A as UInt32", (a, b) => 
                a.MayParseUInt32());
            
            Show("A as Double", (a, b) => 
                a.MayParseDouble());

            Show("B as Double else NaN", (a, b) =>
                b.MayParseDouble()
                .Else(Double.NaN));

            Show("A + B (BigInteger)", (a, b) => 
                from va in a.MayParseBigInteger() 
                from vb in b.MayParseBigInteger() 
                select va + vb);
            
            Show("A * B (BigInteger)", (a, b) => 
                a.MayParseBigInteger()
                .Combine(
                    b.MayParseBigInteger(), 
                    (va, vb) => va * vb));

            Show("SquareRoot(A)", (a, b) =>
                a.MayParseDouble()
                .Select(e => e.MaySqrt())
                .Unwrap());

            Show("SquareRoot(B)", (a, b) =>
                b.MayParseDouble()
                .Bind(e => e.MaySqrt()));

            Show("SquareRoot(A) - SquareRoot(B)", (a, b) =>
                from va in a.MayParseDouble()
                from vb in b.MayParseDouble()
                from sa in va.MaySqrt()
                from sb in vb.MaySqrt()
                select sa - sb);

            Show("First double in [A, B]", (a, b) =>
                new[] { a, b }
                .Select(e => e.MayParseDouble())
                .WhereHasValue()
                .MayFirst());

            Show("[A, B] as IEnumable<double>", (a, b) =>
                new[] { a, b }
                .Select(e => e.MayParseDouble())
                .MayAll()
                .Select(e => "[" + String.Join(", ", e) + "]"));

            var passes = 0;
            var fails = 0;
            Show("# of times A was a double", (a, b) => {
                a.MayParseDouble()
                    .IfHasValueThenDo(ignoredValue => passes += 1)
                    .ElseDo(() => fails += 1);
                return passes + "/" + (passes + fails);
            });

            var sum = BigInteger.Zero;
            Show("Sum of B each time it was a BigInteger", (a, b) => {
                b.MayParseBigInteger().IfHasValueThenDo(v => sum += v);
                return sum;
            });
        }

        private void Show(string title, Func<string, string, object> computation) {
            // create controls to show result
            var rowColor = Rows.Children.Count % 2 == 0 ? Color.FromArgb(0, 0, 0, 0) : Color.FromArgb(25, 0, 0, 0);
            var rowStack = new StackPanel { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(rowColor) };
            var titleBlock = new TextBlock { Width = 300, Text = title };
            var valueBlock = new TextBlock();
            rowStack.Children.Add(titleBlock);
            rowStack.Children.Add(valueBlock);
            Rows.Children.Add(rowStack);

            // show initial value
            Action update = () => valueBlock.Text = "" + computation(txtA.Text, txtB.Text);
            update();

            // recompute when text changes
            TextChangedEventHandler h = (sender, arg) => update();
            txtA.TextChanged += h;
            txtB.TextChanged += h;
        }
    }
}
