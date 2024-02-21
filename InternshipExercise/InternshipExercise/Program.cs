using InternshipExercise;

public class Program
{
    public static void Main(string[] args)
    {
        GeometricShape circle = new Circle();
        GeometricShape triangle = new Triangle();

        Console.WriteLine($"Area of a Circle With the Radius of 5 is : {circle.GetArea(5)};\n");
        Console.WriteLine($"Area of a Triangle With the Sides of 3, 4, 5 : {triangle.GetArea(3, 4, 5)};\n");
        Console.WriteLine($"Area of a Triangle With the Sides of 5, 5, 6 : {triangle.GetArea(5, 5, 6)};\n");
        Console.WriteLine($"Area of a Triangle With the Sides of 7, 7, 7 : {triangle.GetArea(7, 7, 7)};\n");
    }
}