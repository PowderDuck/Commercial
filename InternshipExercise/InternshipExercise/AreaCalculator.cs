using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace InternshipExercise
{
    //Exercise 1;
    public interface GeometricShape
    {
        public float GetArea(params float[] geometricValues);
    }

    public class Circle : GeometricShape
    {
        public float GetArea(params float[] geometricValues)
        {
            if (geometricValues.Length < 1f)
                throw new Exception("Insufficient number of arguments, at least 1 needed !");

            float radius = geometricValues[0];
            return MathF.PI * radius * radius;
        }
    }

    public class Triangle : GeometricShape
    {
        private readonly string[] triangleTypes = new string[] { "Common", "Isosceles", "Equilateral" };
 
        public float GetArea(params float[] geometricValues)
        {
            if (geometricValues.Length < 3f)
                throw new Exception("Insufficient number of arguments; at least 3 needed !");

            int typeIndex = 1;
            float a = geometricValues[0];
            float b = geometricValues[1];
            float c = geometricValues[2];

            for (int i = 0; i < 3; i++)
            {
                float currentSide = geometricValues[i];
                typeIndex = 1;

                for (int x = 0; x < 2; x++)
                {
                    typeIndex += currentSide == geometricValues[(i + x + 1) % 3] ? 1 : 0;
                }

                if(typeIndex > 1f)
                {
                    break;
                }
            }

            float semiPerimeter = (a + b + c) / 2f;
            float area = MathF.Sqrt(semiPerimeter * (semiPerimeter - a) * (semiPerimeter - b) * (semiPerimeter - c));
            Console.WriteLine($"{triangleTypes[typeIndex - 1]} Triangle;");

            return area;
        }
    }

    //Exercise 2;
    /*
     
     CREATE TABLE Products(
        ProductID INT PRIMARY KEY, 
        ProductName VARCHAR(255) NOT NULL, 
        Category VARCHAR(255)
     );

     SELECT ProductName, Category FROM Products;

     */
}
