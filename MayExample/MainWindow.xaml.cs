using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Strilanc.Value;

namespace MayExample {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

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

            Show("A + B (BigInteger)", (a, b) => 
                from va in a.MayParseBigInteger() 
                from vb in b.MayParseBigInteger() 
                select va + vb);
            
            Show("A * B (BigInteger)", (a, b) => 
                a.MayParseBigInteger().Combine(
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
                new[] { a, b }.Select(e => e.MayParseDouble())
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
    internal static class MayUtil {
        ///<summary>Returns the signed 32bit integer represented by the given string, if there is one.</summary>
        public static May<int> MayParseInt32(this string text) {
            int result;
            if (!Int32.TryParse(text, out result)) return May.NoValue;
            return result;
        }
        ///<summary>Returns the unsigned 32bit integer represented by the given string, if there is one.</summary>
        public static May<uint> MayParseUInt32(this string text) {
            uint result;
            if (!UInt32.TryParse(text, out result)) return May.NoValue;
            return result;
        }
        ///<summary>Returns the double represented by the given string, if there is one.</summary>
        public static May<double> MayParseDouble(this string text) {
            double result;
            if (!Double.TryParse(text, out result)) return May.NoValue;
            return result;
        }
        ///<summary>Returns the big integer represented by the given string, if there is one.</summary>
        public static May<BigInteger> MayParseBigInteger(this string text) {
            BigInteger result;
            if (!BigInteger.TryParse(text, out result)) return May.NoValue;
            return result;
        }

        ///<summary>Returns the non-negative square root of the given value, unless no real solution exists.</summary>
        public static May<double> MaySqrt(this double value) {
            if (value >= 0) return Math.Sqrt(value);
            return May.NoValue;
        }

        ///<summary>Returns the first item in the sequence, unless it's empty.</summary>
        public static May<T> MayFirst<T>(this IEnumerable<T> sequence) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            using (var e = sequence.GetEnumerator())
                if (e.MoveNext())
                    return e.Current;
            return May.NoValue;
        }

        /// <summary>
        /// Enumerates the values in the potential values in the sequence.
        /// The potential values that contain no value are skipped.
        /// </summary>
        public static IEnumerable<T> WhereHasValue<T>(this IEnumerable<May<T>> sequence) {
            return sequence.Where(e => e.HasValue).Select(e => (T)e);
        }
        /// <summary>
        /// Returns a sequence of the actual values in the potential values in the sequence.
        /// However, if any of the potential values contains no value then the result is no value.
        /// </summary>
        public static May<IEnumerable<T>> MayAll<T>(this IEnumerable<May<T>> sequence) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            var result = new List<T>();
            foreach (var potentialValue in sequence)
                if (!potentialValue.IfHasValueThenDo(result.Add).HasValue)
                    return May.NoValue;
            return result;
        }
    }
}
