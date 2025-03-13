using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homework2
{
    public interface IShape
    {
        double CalculateArea();
        bool IsValid();
    }

    public abstract class Shape : IShape
    {
        public abstract double CalculateArea();
        public abstract bool IsValid();
    }

    public class Rectangle : Shape
    {
        private double _width;
        private double _height;

        public double Width
        {
            get { return _width; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Width must be greater than zero.");
                }
                _width = value;
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Height must be greater than zero.");
                }
                _height = value;
            }
        }

        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public override double CalculateArea()
        {
            return Width * Height;
        }

        public override bool IsValid()
        {
            return Width > 0 && Height > 0;
        }
    }

    public class Square : Rectangle
    {

        public new double Width
        {
            get { return base.Width; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Width must be greater than zero.");
                }
                base.Width = value;
                base.Height = value;
            }
        }

        public new double Height
        {
            get { return base.Height; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Height must be greater than zero.");
                }
                base.Width = value;
                base.Height = value;
            }
        }

        public Square(double side) : base(side, side)
        {
        }
    }


    public class Triangle : Shape
    {

        private double _baseLength;
        private double _height;

        public double BaseLength
        {
            get { return _baseLength; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Base length must be greater than zero.");
                }
                _baseLength = value;
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Height must be greater than zero.");
                }
                _height = value;
            }
        }

        public Triangle(double baseLength, double height)
        {
            BaseLength = baseLength;
            Height = height;
        }

        public override double CalculateArea()
        {
            return 0.5 * BaseLength * Height;
        }

        public override bool IsValid()
        {
            return BaseLength > 0 && Height > 0;
        }
    }
    public class ShapeFactory
    {
        private static readonly Random random = new Random();

        public static IShape CreateShape()
        {
            int shapeType = random.Next(0, 3); 

            switch (shapeType)
            {
                case 0:
                    double width = random.NextDouble() * 10; 
                    double height = random.NextDouble() * 10; 
                    return new Rectangle(width, height);
                case 1:
                    double side = random.NextDouble() * 10;
                    return new Square(side);
                case 2:
                    double baseLength = random.NextDouble() * 10; 
                    double heightTriangle = random.NextDouble() * 10;
                    return new Triangle(baseLength, heightTriangle);
                default:
                    throw new ArgumentException("Invalid shape type."); 
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Rectangle rectangle = new Rectangle(5, 10);
                Console.WriteLine($"Rectangle Area: {rectangle.CalculateArea()}, Is Valid: {rectangle.IsValid()}");

                Square square = new Square(7);
                Console.WriteLine($"Square Area: {square.CalculateArea()}, Is Valid: {square.IsValid()}");

                Triangle triangle = new Triangle(6, 8);
                Console.WriteLine($"Triangle Area: {triangle.CalculateArea()}, Is Valid: {triangle.IsValid()}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
