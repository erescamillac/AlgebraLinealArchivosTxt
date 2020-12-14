using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Reflection;

namespace AlgebraLinealArchivosTxt
{
    //##++ INI: Vectores y Matrices
    //--Custom Exceptions
    public class NotDefinedMatrixOperationException : Exception
    {
        public NotDefinedMatrixOperationException()
        {
        }

        public NotDefinedMatrixOperationException(string message) : base(message)
        {
        }

        public NotDefinedMatrixOperationException(string message, Exception inner) : base(message, inner)
        {
        }
    }//--fin: NotDefinedMatrixOperationException
    public class NotDefinedVectorOperationException : Exception
    {
        public NotDefinedVectorOperationException()
        {
        }

        public NotDefinedVectorOperationException(string message) : base(message)
        {
        }

        public NotDefinedVectorOperationException(string message, Exception inner) : base(message, inner)
        {
        }
    }//--fin: NotDefinedVectorOperationException

    public class NotInitializedEECException : Exception
    {
        public NotInitializedEECException()
        {
        }

        public NotInitializedEECException(string message) : base(message)
        {
        }

        public NotInitializedEECException(string message, Exception inner) : base(message, inner)
        {
        }
    }//--fin: NotInitializedEECException

    //-- TODO: Implement InputReader
    public class InputReader
    {
        public InputReader()
        {
        }

        // ReadMatrixData : Implementación OK
        public void ReadMatrixData(Matrix matrixA)
        {
            RowVector rVectorTmp;
            (int mRows, int nCols) size;
            if (matrixA is null)
            {
                Console.WriteLine("ERROR en procesamiento de datos, se debe especificar una Matriz NO nula para proceder al ingreso de datos desde el teclado, por parte del usuario.");
            }
            else
            {
                size = matrixA.GetSize();
                // POR CADA Renglón leer
                for (int iRow = 0; iRow < size.mRows; iRow++)
                {
                    rVectorTmp = InputReader.ReadRowVector(iRow + 1, size.nCols);
                    matrixA.SetRow(iRow, rVectorTmp);
                }
            }
        }//--fin: ReadMatrixData

        //#ReadInt() : Implementación CORRECTA
        public int ReadInt(string inputMsg, string errorMsg, bool readOneChar)
        {
            string valueAsString;
            bool successParsing = true;
            int num = -1;
            do
            {
                if (!successParsing)
                {
                    Console.Write(errorMsg);
                    if (readOneChar)
                    {
                        Console.Write(" El número DEBE SER un ENTERO NO NEGATIVO (0, 1, 2, etc.).\n");
                    }
                }
                Console.Write("\n" + inputMsg);
                if (readOneChar)
                {
                    valueAsString = Console.ReadKey().KeyChar.ToString();
                }
                else
                {
                    valueAsString = Console.ReadLine();
                }
                successParsing = Int32.TryParse(valueAsString, out num);
            } while (!successParsing);
            return num;
        }//--fin: ReadInt

        // unit: System.UInt32
        // forza la lectura de un uint > 0 (1, 2, 3) [x es Natural]
        // destinado a lectura del Número de filas y Número de columnas para las Matrices.
        public uint ReadSize(string inputMsg)
        {
            uint size = 0;
            bool succesParsing = true;
            string valueAsString, errorMsg;
            errorMsg = "";
            do
            {
                if (!succesParsing)
                {
                    Console.WriteLine(errorMsg);
                }

                Console.Write(inputMsg);
                valueAsString = Console.ReadLine();

                succesParsing = UInt32.TryParse(valueAsString, out size);
                if (succesParsing)
                {
                    if (size > 0)
                    {
                        succesParsing = true;
                    }
                    else
                    {
                        succesParsing = false;
                        errorMsg = $"El valor ingresado DEBE SER un entero MAYOR QUE 0 (1, 2, 3, ...). El valor ingresado [{size}] NO ES MAYOR que 0.";
                    }
                }
                else
                {
                    errorMsg = $"Debe ingresar un valor ENTERO NO NEGATIVO, la línea ingresada [{valueAsString}] NO REPRESENTA un número ENTERO NO NEGATIVO.";
                }
            } while (!succesParsing);
            return size;
        }//--fin: ReadSize

        // mejora en la implementación de .ReadRowVector
        public static RowVector ReadRowVector(int iRow, int nCols)
        {
            RowVector rVector = null;
            bool error = false, successfulParsing;
            double doubleTmp;
            double[] unidimensionalArray = null;
            string errorMsg = "", lineFromKeyboard;
            string[] individualValues;
            int idxTmp;
            do
            {
                if (error)
                {
                    Console.WriteLine(errorMsg);
                }
                error = false;
                Console.WriteLine("Ingrese los valores del Renglón [{0}], separados por COMAS (por ej. 3, 2.5, 9). Se esperan ({1}) valores: ", iRow, nCols);
                lineFromKeyboard = Console.ReadLine();
                lineFromKeyboard = lineFromKeyboard.Trim();
                if (lineFromKeyboard is null)
                {
                    error = true;
                    errorMsg = "Se debe ingresar por lo menos 1 valor.";
                }
                else
                {
                    if (lineFromKeyboard.Length == 0)
                    {
                        error = true;
                        errorMsg = "Se debe ingresar por lo menos 1 número.";
                    }
                    else
                    {
                        individualValues = lineFromKeyboard.Split(',');
                        // validar concordancia 1 a 1
                        if (individualValues.Length == nCols)
                        {
                            // OK, correpondencia 1 a 1
                            unidimensionalArray = new double[nCols];
                            idxTmp = -1;
                            // validar conversiones a double
                            foreach (string value in individualValues)
                            {
                                idxTmp++;
                                successfulParsing = Double.TryParse(value, out doubleTmp);
                                if (!successfulParsing)
                                {
                                    error = true;
                                    errorMsg = $"El valor [{value}] NO representa un número de punto flotante. POR FAVOR, ingrese valores como (1, 3.5, 8, 2, 3.32, etc.).";
                                    break;
                                }
                                else
                                {
                                    // conversión a Double OK
                                    unidimensionalArray[idxTmp] = doubleTmp;
                                }
                            }
                        }
                        else
                        {
                            // ERROR: inconsistencia entre el número de valores ingresador y esperados.
                            error = true;
                            errorMsg = $"El número de valores ingresados desde el teclado ({individualValues.Length}) NO corresponde con el número de valores esperados ({nCols}).";
                        }
                    }
                }
            } while (error);
            rVector = new RowVector((uint)nCols);
            rVector.SetValues(unidimensionalArray);
            return rVector;
        }//--fin ReadRowVectorv2

        // Implementación correcta.
        public double[] ReadDoubleArray(int nElements)
        {
            double[] result = null;
            string[] individualValues = null;
            string valueAsString, errorMsg;
            bool error = false, successfulParsing;
            int numValIngresados, indxTmp;
            double doubleTmp;
            errorMsg = "";
            do
            {
                if (error)
                {
                    Console.WriteLine(errorMsg);
                }
                error = false;
                if (nElements > 0)
                {
                    Console.WriteLine($"Ingrese {nElements} números separados por comas (por ej. 9, 5, 1.3, 6.57, 2): ");
                    valueAsString = Console.ReadLine();
                    //Console.WriteLine($"Valor leido desde el teclado: [{valueAsString}]");
                    valueAsString = valueAsString.Trim();
                    if (valueAsString is null)
                    {
                        // CADENA null: ERROR
                        error = true;
                        errorMsg = "Se debe ingresar por lo menos un valor.";
                    }
                    else
                    {
                        if (valueAsString.Length == 0)
                        {
                            // EMPTY string : ERROR
                            error = true;
                            errorMsg = "Se debe ingresar por lo menos un número.";
                        }
                        else
                        {
                            individualValues = valueAsString.Split(',');
                            numValIngresados = individualValues.Length;
                            // validar que el número de individualValues sea igual que <nElements>
                            if (individualValues.Length == nElements)
                            {
                                // OK, conconrdancia correcta 1 a 1:
                                result = new double[nElements];
                                indxTmp = -1;
                                // validar conversiones correctas de string a double
                                foreach (string value in individualValues)
                                {
                                    indxTmp++;
                                    successfulParsing = Double.TryParse(value, out doubleTmp);
                                    // Si ocurrio un error de conversión
                                    if (!successfulParsing)
                                    {
                                        error = true;
                                        errorMsg = $"El valor [{value}] NO representa un número de punto flotante. Por favor, ingrese valores como (1.7, 3.0, etc.)";
                                        break;
                                    }
                                    else
                                    {
                                        result[indxTmp] = doubleTmp;
                                    }
                                }
                            }
                            else
                            {
                                // ERROR: Incoherencia entre el número de elementos ingresados y el número de elementos esperados (solicitados).
                                error = true;
                                errorMsg = $"El número de elementos ingresados [{numValIngresados}] NO COINCIDE con el número de elementos esperados [{nElements}]";
                            }
                        }
                    }
                }
                else
                {
                    //ERROR: false para finalizar el ciclo do-while()
                    error = false;
                }
            } while (error);

            return result;
        }//--public double[] ReadDoubleArray

    }//--fin: public class InputReader
    public class FormatterEEC
    {
        public static string FormatDouble(double value, byte decimalPlaces)
        {
            /*
             * Alternativas de implementación:
             *  Console.WriteLine("value a 2 decimales: [{0:f2}]", value);
                Console.WriteLine($"value a 2 decimales: [{value:f2}]");
                Console.WriteLine("Ejemplo de uso de String.Format: [{0}]", String.Format("{0:f2}", value));
                Console.WriteLine(value.ToString("N2"));

                Math.Truncate
                Math.Truncate Method
                https://docs.microsoft.com/en-us/dotnet/api/system.math.truncate?view=net-5.0

             */
            StringBuilder sb = new StringBuilder();
            string s = "";
            sb.Append("N").Append(decimalPlaces);
            s = value.ToString(sb.ToString());
            return s;
        }//-- public static string FormatDouble(double value)
    }

    public class MathUtils
    {

        public static double InversoAditivo(double x)
        {
            return -1.0 * x;
        }//--fin: InversiAditivo
        public static double InversoMultiplicativo(double x)
        {
            if (x == 0)
            {
                throw new ArgumentException($"El inverso multiplicativo de {x} NO ESTÁ DEFINIDO");
            }
            else
            {
                return Math.Pow(x, -1);
            }
        }
    }//--fin: class mathUtils
    public class ArrayUtils
    {

        public static void FillWithValue(double[,] bidimensionalArray, double value)
        {
            (int mRows, int nCols) size;
            if (!(bidimensionalArray is null))
            {
                size = GetSize(bidimensionalArray);
                for (int i = 0; i < size.mRows; i++)
                {
                    for (int j = 0; j < size.nCols; j++)
                    {
                        bidimensionalArray[i, j] = value;
                    }
                }
            }
        }//--fin: FillWithValues

        // Devuelve el número de dimensiones del arreglo <array>
        // -1: representa que el arreglo <array> pasado como argumento fue null.
        public static int GetDimensions(Array array)
        {
            int nDimensions = -1;
            if (!(array is null))
            {
                nDimensions = array.Rank;
            }
            return nDimensions;
        } //-- fin: GetDimensions()

        public static void ShowContent(Array array)
        {
            int nDimensions = -1, numElemento = 0, lastElement;
            (int mRows, int nCols) size;
            if (array is null)
            {
                Console.WriteLine("¡El arreglo es NULL!");
            }
            else
            {
                // 1. Determinar el número de dimensiones del arreglo
                nDimensions = GetDimensions(array);
                // Si se trata de un arreglo Bidimensional -->> mostrar en forma Renglones - Columnas
                if (nDimensions == 2)
                {
                    if (array.GetType() == typeof(double[,]))
                    {
                        // determinar tamaño del arreglo Bidimensional
                        size = GetSize((double[,])array);
                        // por cada Renglón hacer:
                        for (int i = 0; i < size.mRows; i++)
                        {
                            Console.Write("\n");
                            // si es la PRIMERA FILA. (PRIMER Renglón)
                            if (i == 0)
                            {
                                Console.Write("[");
                            }
                            for (int j = 0; j < size.nCols; j++)
                            {
                                // si NO es la última posición
                                if (j != size.nCols - 1)
                                {
                                    //Console.Write(array.GetValue(i, j) + "\t");
                                    Console.Write(FormatterEEC.FormatDouble((double)array.GetValue(i, j), 2) + "\t");
                                }
                                else
                                {
                                    Console.Write(FormatterEEC.FormatDouble((double)array.GetValue(i, j), 2));
                                    //Console.Write(array.GetValue(i, j));
                                }
                            }

                        }//-- fin: recorrido de renglones (i-ésimos) del array Bidimensional
                        Console.Write("]\n");
                    } //-- typeof(double[,])

                }
                else
                {
                    // para cualquier otra dimension (unidimensionales, 3D, 4D, etc.)
                    // simplemente usar foreach
                    // First element : 1
                    lastElement = array.Length;
                    Console.Write("\n[");
                    foreach (var element in array)
                    {
                        numElemento++;
                        if (numElemento != lastElement)
                        {
                            Console.Write(element + "\t");
                        }
                        else
                        {
                            Console.Write(element);
                        }
                    }//--foreach
                    Console.Write("]\n");
                }
            }
        }//-- ShowContent

        public static void ShowContent(double[] unidimensionalArray, string separator)
        {
            // Console.WriteLine("Dentro del método ShowContent(double[])");
            int lastIndex, idxTmp;
            if (unidimensionalArray is null)
            {
                Console.WriteLine("¡El arreglo especificado es NULL!");
            }
            else
            {
                lastIndex = unidimensionalArray.GetUpperBound(0);
                idxTmp = -1;
                Console.Write("\n[");
                foreach (double element in unidimensionalArray)
                {
                    idxTmp++;
                    Console.Write(FormatterEEC.FormatDouble(element, 2));
                    if (idxTmp != lastIndex)
                    {
                        Console.Write(separator);
                    }
                }
                Console.Write("]\n");
            }
        }//-- ShowContent() 

        public static string ToBeautyString(double[] unidimensionalArray, string separator)
        {
            StringBuilder sb = new StringBuilder();
            int lastIndex, idxTmp;
            if (unidimensionalArray is null)
            {
                sb.Append("¡El arreglo es NULL!");
            }
            else
            {
                idxTmp = -1;
                lastIndex = unidimensionalArray.GetUpperBound(0);
                sb.Append("[");
                foreach (double element in unidimensionalArray)
                {
                    idxTmp++;
                    sb.Append(FormatterEEC.FormatDouble(element, 2));
                    if (idxTmp != lastIndex)
                    {
                        sb.Append(separator);
                    }
                }
                sb.Append("]");
            }
            return sb.ToString();
        }//-- ToBeautyString()

        public static string ToBeautyString(double[,] doubleArray)
        {
            StringBuilder sb = new StringBuilder();
            (int mRows, int nCols) size;
            if (!(doubleArray is null))
            {
                size = GetSize(doubleArray);
                // Renglón i-ésimo (i)
                for (int i = 0; i < size.mRows; i++)
                {
                    sb.Append("\n");
                    // Si es la PRIMERA FILA
                    if (i == 0)
                    {
                        sb.Append("[");
                    }
                    // COL j-ésima (j)
                    for (int j = 0; j < size.nCols; j++)
                    {
                        sb.Append(FormatterEEC.FormatDouble(doubleArray[i, j], 2));
                        // si NO es la ÚLTIMA Columna
                        if (j != size.nCols - 1)
                        {
                            sb.Append("\t");
                        }
                    }
                }//-- fin: Recorrido de RENGLONES (i-ésimo)
                sb.Append("]\n");
            }
            return sb.ToString();
        }//-- fin: ToBeautyString(double[,] doubleArray)

        public static (int mRows, int nCols) GetSize(double[,] doubleArray)
        {
            (int mRows, int nCols) size = (-1, -1);
            if (!(doubleArray is null))
            {
                // Dimension '0' : Filas
                size.mRows = doubleArray.GetLength(0);
                // Dimension '1' : Columnas
                size.nCols = doubleArray.GetLength(1);
            }
            return size;
        }// m x n size

        public static int NumberOfRows(double[,] doubleArray)
        {
            int numberOfRows = -1;
            (int mRows, int nCols) size;
            if (!(doubleArray is null))
            {
                size = GetSize(doubleArray);
                numberOfRows = size.mRows;
            }
            return numberOfRows;
        }//-- NumberOfRows

        public static int NumberOfCols(double[,] doubleArray)
        {
            int numberOfCols = -1;
            (int mRows, int nCols) size;
            if (!(doubleArray is null))
            {
                size = GetSize(doubleArray);
                numberOfCols = size.nCols;
            }
            return numberOfCols;
        }//-- public static int NumberOfCols

    }//-- public class ArrayUtils

    /*CRC: Class Rensponsability
     * Matrix class
     * 1.- El innerArray (double[,]) NO puede modificarse en SU TOTALIDAD de forma completa
     * matrixA.InnerArray = new double[2, 5]; //*NO SE PUEDE PERMITIR
     * 2.- (Internal) - private modificación RENGLÓN a RENGLÓN // quizá COL a COL
     * 3.- Implementar método .clone() <.Clone(): return new Matrix(m, n) ${con igual elementos} ${CLONAR .InnerArray}>
     * 4.- "GetInnerArrayClone" ${devolver un CLON del InnerArray}
     */

    /*
     * #Abstract class: Vector
     */
    public abstract class Vector
    {
        protected double[] _innerArray;

        public abstract double[,] ToBidimensionalArray();
        //public abstract Vector MultiplicacionPorEscalar(double escalar);

        public bool SetValues(double[] values)
        {
            bool success = true;
            if (values is null)
            {
                success = false;
            }
            else
            {
                if (_innerArray.Length == values.Length)
                {
                    _innerArray = values;
                    success = true;
                }
                else
                {
                    success = false;
                }
            }
            return success;
        }//--fin: SetValues()

        public bool SetValueAt(int idx, double value)
        {
            bool success = false;
            if (idx >= 0 && idx < _innerArray.Length)
            {
                _innerArray[idx] = value;
                success = true;
            }
            return success;
        }//-- SetValueat

        public double GetValueAt(int idx)
        {
            double value = -1.1;
            if (idx >= 0 && idx < _innerArray.Length)
            {
                value = _innerArray[idx];
            }
            return value;
        }//-- GetValueAt

        // Producto punto
        // DotProduct(Vector v2) : Implementación CORRECTA. [OK]
        public double DotProduct(Vector v2)
        {
            double result = -1.1;
            int n1, n2, indxTmp;
            if (v2 is null)
            {
                throw new ArgumentNullException("No se puede calcular el Producto punto con un vector NULL.");
            }
            else
            {
                // Validar igual dimension
                n1 = this.Size();
                n2 = v2.Size();
                if (n1 == n2)
                {
                    // OK, se puede continuar con la operación, la operación Producto punto esta DEFINIDA
                    // usar result como acumulador
                    result = 0.0;
                    indxTmp = -1;
                    foreach (double compV1 in _innerArray)
                    {
                        indxTmp++;
                        result += compV1 * v2.GetValueAt(indxTmp);
                    }
                }
                else
                {
                    // SI los vectores NO son del mismo tamaño
                    throw new NotDefinedVectorOperationException($"No es posible calcular el Producto Punto entre un vector de {n1} componentes y otro vector de {n2} componentes.");
                }
            }
            return result;
        }//-- DotProduct

        // IsAllZeros : Implementación CORRECTA - [OK]
        public bool IsAllZeros()
        {
            bool isAllZeros = true;
            if (_innerArray is null)
            {
                throw new NotInitializedEECException("Vector AÚN NO inicializado!");
            }
            else
            {
                foreach (double element in _innerArray)
                {
                    if (element != 0)
                    {
                        isAllZeros = false;
                        break;
                    }
                }
            }
            return isAllZeros;
        }//--fin: IsAllZeros

        public int Size()
        {
            int nElem = -1;
            if (!(_innerArray is null))
            {
                nElem = _innerArray.Length;
            }
            return nElem;
        }

    }//--fin: abstract class Vector

    // Column vector array[n, 1]
    public class ColVector : Vector
    {
        public ColVector(uint n)
        {
            _innerArray = new double[n];
        }

        public ColVector MultiplicacionPorEscalar(double escalar)
        {
            ColVector vector = null;
            int indxTmp;
            if (!(_innerArray is null))
            {
                vector = new ColVector((uint)_innerArray.Length);
                indxTmp = -1;
                foreach (double element in _innerArray)
                {
                    indxTmp++;
                    vector.SetValueAt(indxTmp, element * escalar);
                }
            }
            return vector;
        }

        public override double[,] ToBidimensionalArray()
        {
            double[,] bidimensionalArray = null;
            int i;
            if (!(_innerArray is null))
            {
                bidimensionalArray = new double[_innerArray.Length, 1];
                i = -1;
                foreach (double element in _innerArray)
                {
                    i++;
                    bidimensionalArray[i, 0] = element;
                }
            }
            return bidimensionalArray;
        }

        public override string ToString()
        {
            string s = "";
            s = ArrayUtils.ToBeautyString(_innerArray, "\n");
            return s;
        }

    }//--fin: public class ColVector

    /*
     * VectorRenglon
     * RowVector array[1, n]
     */
    // public class RowVector
    public class RowVector : Vector
    {
        public RowVector(uint n)
        {
            _innerArray = new double[n];
        }

        public RowVector AddRowVector(RowVector otherRowV)
        {
            RowVector result = null;
            int ownSize, otherSize;
            if (!(otherRowV is null))
            {
                // mismo tamaño
                ownSize = this.Size();
                otherSize = otherRowV.Size();
                if (ownSize != otherSize)
                {
                    // si NO son de mismo tamaño, la SUMA de Vectores NO está Definida
                    throw new NotDefinedVectorOperationException($"La suma de Vectores RENGLÓN NO esta definida para distinto numero de componentes, V1 tiene {ownSize} componentes y V2 tiene {otherSize} componentes.");
                }
                else
                {
                    result = new RowVector((uint)ownSize);
                    for (int jCol = 0; jCol < ownSize; jCol++)
                    {
                        result.SetValueAt(jCol, this.GetValueAt(jCol) + otherRowV.GetValueAt(jCol));
                    }
                }
            }
            else
            {
                // otherRowV is null
                throw new ArgumentNullException("No se puede evaluar la SUMA de vectores Renglón con argumento NULL.");
            }
            return result;
        }//--fin: AddRowVector

        public RowVector MultiplicacionPorEscalar(double escalar)
        {
            RowVector rowV = null;
            int indxTmp;
            if (!(_innerArray is null))
            {
                rowV = new RowVector((uint)_innerArray.Length);
                indxTmp = -1;
                foreach (double component in _innerArray)
                {
                    indxTmp++;
                    rowV.SetValueAt(indxTmp, component * escalar);
                }
            }
            return rowV;
        }//--fin: MultiplicacionPorEscalar

        public override double[,] ToBidimensionalArray()
        {
            double[,] bidimensionalArray = null;
            int lastIndex, indxTmp;
            if (!(_innerArray is null))
            {
                bidimensionalArray = new double[1, _innerArray.Length];
                lastIndex = _innerArray.GetUpperBound(0);
                indxTmp = -1;
                foreach (double element in _innerArray)
                {
                    indxTmp++;
                    bidimensionalArray[0, indxTmp] = element;
                }
            }
            return bidimensionalArray;
        }//--fin: ToBidimensionalArray

        public override string ToString()
        {
            string s = "";
            s = ArrayUtils.ToBeautyString(_innerArray, "\t");
            return s;
            //return base.ToString();
        }
    }//--fin: RowVector

    //--Matrix class
    public class Matrix
    {
        private double[,] _inner2DArray;

        public Matrix(int mRows, int nCols)
        {
            if (mRows <= 0)
            {
                throw new ArgumentException($"El número de renglones para la creación de una Matriz debe ser por lo menos 1. El número de filas especificado {mRows} ES INVÁLIDO.");
            }
            if (nCols <= 0)
            {
                throw new ArgumentException($"El número de Columnas para la creación de una Matriz debe ser por lo menos 1. El número de columnas especificado {nCols} ES INVÁLIDO.");
            }
            _inner2DArray = new double[mRows, nCols];
        }

        // setrow : OK
        public bool SetRow(int iRowPos, RowVector rVector)
        {
            bool success = false;
            int numComponents, nCols;
            (int mRows, int nCols) size;
            if (!(this.IsValidRowPos(iRowPos)))
            {
                // posición de fila NO válida
                throw new IndexOutOfRangeException($"La posición de RENGLÓN ({iRowPos}) NO es VÁLIDA para la Matriz.");
            }
            else
            {
                if (rVector is null)
                {
                    throw new ArgumentNullException($"El vector Renglón que se debe insertar en la Matriz NO puede ser null.");
                }
                else
                {
                    numComponents = rVector.Size();
                    size = this.GetSize();
                    nCols = size.nCols;
                    if (numComponents != nCols)
                    {
                        // ERROR de dimensión del RowVector pasado como argumento al momento de invocar el método
                        throw new IndexOutOfRangeException($"El número de componentes del Vector RENGLÓN ({numComponents}) NO CONCUERDA con el número de COLUMNAS en la Matriz ({nCols}).");
                    }
                    else
                    {
                        //OK : realizar asignación.
                        // renglón: iRowPos, col (jCol)
                        for (int jCol = 0; jCol < size.nCols; jCol++)
                        {
                            _inner2DArray[iRowPos, jCol] = rVector.GetValueAt(jCol);
                        }
                        success = true;
                    }
                }//--fin: else
            }
            return success;
        }//-- setRow

        // validación de Indexes
        public double GetValueAt(int iRow, int jCol)
        {
            if (!(this.IsValidRowPos(iRow)))
            {
                throw new IndexOutOfRangeException($"La fila {iRow} NO es VÁLIDA para la matriz.");
            }
            if (!(this.IsValidColPos(jCol)))
            {
                throw new IndexOutOfRangeException($"La COLUMNA {jCol} NO es VÁLIDA para la matriz.");
            }
            return _inner2DArray[iRow, jCol];
        }

        // validación de Indexes
        public void SetValueAt(int iRow, int jCol, double value)
        {
            if (!(this.IsValidRowPos(iRow)))
            {
                throw new IndexOutOfRangeException($"La fila {iRow} NO es VÁLIDA para la matriz.");
            }
            if (!(this.IsValidColPos(jCol)))
            {
                throw new IndexOutOfRangeException($"La COLUMNA {jCol} NO es VÁLIDA para la matriz.");
            }
            _inner2DArray[iRow, jCol] = value;
        }

        // recuperar Renglón i-ésimo : OK
        public RowVector GetRow(int iRow)
        {
            RowVector rVector = null;
            (int mRows, int nCols) size;
            if (!(this.IsValidRowPos(iRow)))
            {
                throw new IndexOutOfRangeException($"La fila especificada {iRow} NO es válida para la matriz.");
            }
            size = this.GetSize();
            rVector = new RowVector((uint)size.nCols);
            for (int jCol = 0; jCol < size.nCols; jCol++)
            {
                rVector.SetValueAt(jCol, _inner2DArray[iRow, jCol]);
            }
            return rVector;
        }//--fin: GetRow

        // recuperar Columna j-ésima : OK
        public ColVector GetCol(int jCol)
        {
            ColVector cVector = null;
            (int mRows, int nCols) size;
            if (!(this.IsValidColPos(jCol)))
            {
                throw new IndexOutOfRangeException($"La COLUMNA {jCol} NO es VÁLIDA para la matriz.");
            }
            size = this.GetSize();
            cVector = new ColVector((uint)size.mRows);
            for (int iRow = 0; iRow < size.mRows; iRow++)
            {
                cVector.SetValueAt(iRow, _inner2DArray[iRow, jCol]);
            }
            return cVector;
        }//--fin: GetCol

        // multiplicación por escalar

        // clonar matriz

        // Inversa de matriz

        public bool SetValues(double[,] values)
        {
            bool success = false;
            (int mRows, int nCols) valuesSize, ownSize;
            if (!(values is null))
            {
                valuesSize = ArrayUtils.GetSize(values);
                ownSize = this.GetSize();
                //Console.WriteLine("Matrix own size: [{0}]", ownSize);
                //Console.WriteLine("2dArray <values> size: [{0}]", valuesSize);
                // si values tiene el mismo tamaño que _inner2DArray
                // ownSize VS. valuesSize
                if ((ownSize.mRows == valuesSize.mRows) && (ownSize.nCols == valuesSize.nCols))
                {
                    _inner2DArray = values;
                    success = true;
                }
            }//-- !(values is null)
            return success;
        }

        public (int mRows, int nCols) GetSize()
        {
            return ArrayUtils.GetSize(_inner2DArray);
        }

        public bool IsValidRowPos(int rowPos)
        {
            bool isValid;
            (int mRows, int nCols) size;
            size = this.GetSize();
            if ((rowPos >= 0) && (rowPos < size.mRows))
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }
            return isValid;
        }

        public bool IsValidColPos(int colPos)
        {
            bool isValid;
            (int mRows, int nCols) size;
            size = this.GetSize();
            if ((colPos >= 0) && (colPos < size.nCols))
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }
            return isValid;
        }

        // MULTIPLICACIÓN por otra matriz. : Implementación OK.
        public Matrix MultiplicacionPorMatriz(Matrix B)
        {
            Matrix resultMatrix = null;
            RowVector rVectorTmp;
            ColVector cVectorTmp;
            double producto;
            (int mRows, int nCols) ownSize, bSize, resultSize;
            if (B is null)
            {
                throw new ArgumentNullException("La matriz B para la multiplicación NO puede ser NULL.");
            }
            else
            {
                // Validar dimensiones de matrices
                ownSize = this.GetSize();
                bSize = B.GetSize();
                // A (m x n); B (n x p); C (m x p)
                if (ownSize.nCols == bSize.mRows)
                {
                    // OK, la operación esta definida
                    // C (m x p)
                    resultMatrix = new Matrix(ownSize.mRows, bSize.nCols);
                    resultSize = resultMatrix.GetSize();
                    for (int iRow = 0; iRow < resultSize.mRows; iRow++)
                    {
                        for (int jCol = 0; jCol < resultSize.nCols; jCol++)
                        {
                            // tomar : (renglón i de A) * (columna j de B)
                            rVectorTmp = this.GetRow(iRow);
                            cVectorTmp = B.GetCol(jCol);
                            //Console.WriteLine("Intentando multiplicar (rowVector)*(colVector)");
                            //Console.WriteLine("\n{0}", rVectorTmp);
                            //Console.WriteLine("\n{0}", cVectorTmp);
                            // dot product
                            producto = rVectorTmp.DotProduct(cVectorTmp);
                            resultMatrix.SetValueAt(iRow, jCol, producto);
                        }
                    }//-- para cada elemento Cij C[i, j]
                }
                else
                {
                    // dimensiones incompatibles, MULTIPLICACION NO definida.
                    throw new NotDefinedMatrixOperationException($"La multiplicacion de una Matriz {ownSize} por una matriz {bSize} NO ESTÁ DEFINIDA. El número de COLUMNAS de A, DEBE SER IGUAL al número de Renglones de B.");
                }
            }
            return resultMatrix;
        }//--fin: MultiplicacionPor Matriz

        // MultiplicarPorEscalar : Implementación OK
        public Matrix MultiplicarPorEscalar(double escalar)
        {
            Matrix matrizResult = null;
            (int mRows, int nCols) size;
            if (_inner2DArray is null)
            {
                throw new NotDefinedMatrixOperationException("La matriz AÚN NO HA sido inicializada.");
            }
            else
            {
                size = this.GetSize();
                matrizResult = new Matrix(size.mRows, size.nCols);
                for (int iRow = 0; iRow < size.mRows; iRow++)
                {
                    for (int jCol = 0; jCol < size.nCols; jCol++)
                    {
                        matrizResult.SetValueAt(iRow, jCol, this.GetValueAt(iRow, jCol) * escalar);
                    }
                }
            }
            return matrizResult;
        }//--fin: MultiplicarPorEscalar()

        //-- Clone
        public Matrix Clone()
        {
            return this.MultiplicarPorEscalar(1);
        }//-- Clone()

        // suma de matrices
        // mismo tamanio
        // AddMatrix : Implementación OK
        public Matrix AddMatrix(Matrix B)
        {
            Matrix result = null;
            (int mRows, int nCols) ownSize, bSize;
            //Console.WriteLine("dentor de Matrix.AddMatrix()...");
            if (B is null)
            {
                throw new ArgumentNullException("NO se puede calcular la suma de matrices con una Matriz NULL.");
            }
            else
            {
                // conitnuar..
                // suma posición a posición
                // validar tamanio (MISMO)
                ownSize = this.GetSize();
                bSize = B.GetSize();
                if ((ownSize.mRows == bSize.mRows) && (ownSize.nCols == bSize.nCols))
                {
                    // son del mismo tamanio: OK, continuar con la operacion
                    // suma posición a posición
                    result = new Matrix(ownSize.mRows, ownSize.nCols);
                    for (int iRow = 0; iRow < ownSize.mRows; iRow++)
                    {
                        for (int jCol = 0; jCol < ownSize.nCols; jCol++)
                        {
                            result.SetValueAt(iRow, jCol, this.GetValueAt(iRow, jCol) + B.GetValueAt(iRow, jCol));
                        }
                    }
                }
                else
                {
                    // matrices de distindo tamanio:  OPERACIÓN NO DEFINIDA
                    throw new NotDefinedMatrixOperationException($"La suma de matrices NO ESTÁ DEFINIDA para matrices de distinto tamaño. No se puede sumar una matriz de {ownSize}, con otra de {bSize}.");
                }
            }
            return result;
        }//--fin: AddMatrix

        // RestarMatrix : Implementación Correcta
        public Matrix RestarMatrix(Matrix B)
        {
            Matrix result = null;
            if (B is null)
            {
                throw new ArgumentNullException("NO es posible calcular la diferencia de matrices para una matriz NULL.");
            }
            else
            {
                result = this.AddMatrix(B.MultiplicarPorEscalar(-1));
            }
            return result;
        }//--fin: .RestarMatrix(Matrix B)

        // resta de matrices

        // determinar Inversa de la Matriz : Implementación OK.
        public Matrix Inverse()
        {
            Matrix inversa = null;
            Matrix parteIzq, parteDer;
            RowVector renglonPivotalIzq, renglonIesimo, renglonIesimoDer, renglonPivotalDer, kRenglonPiv, kRenglonPivDer;
            double valorPivotalTmp, inversoMultiplicativo, k;
            (int mRows, int nCols) size, sizeParteIzq;
            int posArriba, posAbajo;
            // int posPivotal;
            //La Inversa SÓLO ESTA DEFINIDA para una matriz cuadrada (n x n)
            Console.WriteLine("Intentando determinar la Inversa de la Matriz.");
            if (_inner2DArray is null)
            {
                throw new NotDefinedMatrixOperationException("NO se puede determinar la Inversa de una matriz NO inicializada.");
            }
            else
            {
                // Validar que sea una matriz cuadrada.
                size = this.GetSize();
                if (size.mRows != size.nCols)
                {
                    // la Matriz ACTUAL NO es cuadrada
                    throw new NotDefinedMatrixOperationException($"La matriz Inversa NO ESTÁ DEFINIDA para matrices RECTANGULARES [{size}], SÓLO esta definida para matrices CUADRADAS (n x n)");
                }
                else
                {
                    // es Matriz CUADRADA
                    // PASO 1. Escribir matriz Aumentada
                    parteIzq = this.Clone();
                    parteDer = Matrix.IdentityMatrix(this.GetSize().mRows);
                    // PASO 2. Reducción por Renglones en la Parte Izq.
                    // posPivotal = -1;
                    sizeParteIzq = parteIzq.GetSize();
                    for (int posPivotal = 0; posPivotal < sizeParteIzq.mRows; posPivotal++)
                    {
                        // posicion Pivotal [i, i] : [posPivotal, posPivotal]
                        //Console.WriteLine("Posición ({0}, {1}) : [{2}]", posPivotal, posPivotal, parteIzq.GetValueAt(posPivotal, posPivotal));
                        // Fase 1.- Intentar Generar 1 en posición pivotal
                        if (parteIzq.GetValueAt(posPivotal, posPivotal) != 1)
                        {
                            // generar 1 en posición pivotal
                            //Console.WriteLine("Intentado general 1 en posición Pivotal ({0}, {1})...", posPivotal, posPivotal);
                            // determinar Inverso multiplicativo del valor en la posición Pivotal
                            valorPivotalTmp = parteIzq.GetValueAt(posPivotal, posPivotal);
                            if (valorPivotalTmp != 0)
                            {
                                // se puede determinar inverso multiplcativo
                                inversoMultiplicativo = MathUtils.InversoMultiplicativo(valorPivotalTmp);
                                // REESCRIBIR Rpiv --> k*Rpiv
                                // Ri -> k * Ri
                                //Console.WriteLine("R{0} -> {1}*R{0}", posPivotal, inversoMultiplicativo);
                                // Renglón Pivotal (LADO IZQ)
                                renglonPivotalIzq = parteIzq.GetRow(posPivotal);
                                //Console.WriteLine("Renglon Pivotal IZQ: \n{0}", renglonPivotalIzq);
                                renglonIesimo = renglonPivotalIzq.MultiplicacionPorEscalar(inversoMultiplicativo);
                                parteIzq.SetRow(posPivotal, renglonIesimo);
                                //##!!!!--Justo DESPUÉS de .SetRow() en la Parte IZQ. validar Renglón que se acaba --!!!!##
                                //##!!!!--de MODIFICAR para checar si en un Renglón de CEROS (Matriz NO INVERTIBLE) --!!!!##

                                // HOMOLOGAR CAMBIO en PARTE DER.
                                // Ri -> k * Ri
                                renglonPivotalDer = parteDer.GetRow(posPivotal);
                                renglonIesimo = renglonPivotalDer.MultiplicacionPorEscalar(inversoMultiplicativo);
                                parteDer.SetRow(posPivotal, renglonIesimo);

                                // validar estado de la matriz (parte Izq):
                                //Console.WriteLine("Parte IZQ: \n{0}", parteIzq);
                                //Console.WriteLine("--------");
                                //Console.WriteLine("Parte DER: \n{0}", parteDer);

                                // Fase 2.- Generar 0's por Arriba y por Abajo de la Posición pivotal.
                                // Por ARRIBA : recorrido OK
                                //Console.WriteLine("Por ARRIBA:");
                                posArriba = posPivotal;
                                // por Arriba : OK
                                while (parteIzq.IsValidRowPos(--posArriba))
                                {
                                    //Console.WriteLine("posArriba : [{0}]", posArriba);
                                    // posArriba : RENGLÓN
                                    // posPivotal : Col
                                    //Console.WriteLine("Intendando generar 0 en ({0}, {1})...", posArriba, posPivotal);
                                    // Validar valor : Si el valor != 0
                                    if (parteIzq.GetValueAt(posArriba, posPivotal) != 0)
                                    {
                                        // Ri -> Ri + kRpivotal
                                        renglonPivotalIzq = parteIzq.GetRow(posPivotal);
                                        // HOMOLOGAR PARTE DER.
                                        renglonPivotalDer = parteDer.GetRow(posPivotal);
                                        k = MathUtils.InversoAditivo(parteIzq.GetValueAt(posArriba, posPivotal));
                                        // k * Rpivotal
                                        kRenglonPiv = renglonPivotalIzq.MultiplicacionPorEscalar(k);
                                        // HOMOLOGAR PARTE DER.
                                        kRenglonPivDer = renglonPivotalDer.MultiplicacionPorEscalar(k);
                                        renglonIesimo = parteIzq.GetRow(posArriba);
                                        // HOMOLOGAR PARTE DER.
                                        renglonIesimoDer = parteDer.GetRow(posArriba);
                                        renglonIesimo = renglonIesimo.AddRowVector(kRenglonPiv);
                                        // HOMOLOGAR PARTE DER.
                                        renglonIesimoDer = renglonIesimoDer.AddRowVector(kRenglonPivDer);
                                        parteIzq.SetRow(posArriba, renglonIesimo);
                                        // HOMOLOGAR PARTE DER.
                                        parteDer.SetRow(posArriba, renglonIesimoDer);
                                    }
                                }//--fin: Recorrido por ARRIBA

                                // Por ABAJO : recorrido OK
                                //Console.WriteLine("Por ABAJO:");
                                posAbajo = posPivotal;
                                while (parteIzq.IsValidRowPos(++posAbajo))
                                {
                                    //Console.WriteLine("posAbajo : [{0}]", posAbajo);
                                    // posAbajo : RENGLÓN
                                    // posPivotal : Col
                                    //Console.WriteLine("Intentando generar 0 en ({0}, {1})...", posAbajo, posPivotal);
                                    // Validar valor : Si el valor != 0
                                    if (parteIzq.GetValueAt(posAbajo, posPivotal) != 0)
                                    {
                                        // Ri -> Ri + kRPivotal
                                        renglonPivotalIzq = parteIzq.GetRow(posPivotal);
                                        // HOMOLOGAR PARA LA PARTE DER.
                                        renglonPivotalDer = parteDer.GetRow(posPivotal);
                                        k = MathUtils.InversoAditivo(parteIzq.GetValueAt(posAbajo, posPivotal));
                                        // k * Rpivotal
                                        kRenglonPiv = renglonPivotalIzq.MultiplicacionPorEscalar(k);
                                        // HOMOLOGAR PARA LA PARTE DER. 
                                        kRenglonPivDer = renglonPivotalDer.MultiplicacionPorEscalar(k);

                                        renglonIesimo = parteIzq.GetRow(posAbajo);
                                        //HOMOLOGAR PARA LA PARTE DER.
                                        renglonIesimoDer = parteDer.GetRow(posAbajo);
                                        renglonIesimo = renglonIesimo.AddRowVector(kRenglonPiv);
                                        //HOMOLOGAR PARA LA PARTE DER.
                                        renglonIesimoDer = renglonIesimoDer.AddRowVector(kRenglonPivDer);

                                        parteIzq.SetRow(posAbajo, renglonIesimo);
                                        //HOMOLOGAR PARA LA PARTE DER.
                                        parteDer.SetRow(posAbajo, renglonIesimoDer);
                                    }
                                }//--fin: Recorrido POR ABAJO
                            }
                            else
                            {
                                // si el valor es 0, inverso multiplicativo NO ESTÁ DEFINIDO
                                Console.WriteLine("El inverso multiplicativo de ({0}) NO ESTÁ DEFINIDO.", valorPivotalTmp);
                            }
                        }
                    }
                    //--fin: PASO 2. Reducción por Renglones en la Parte Izq.
                    //-- Al final de la Reducción por Renglones
                    //-- Paso 3. Decidir si A es invertible?
                    Console.WriteLine("Al final de la Reducción por Renglones...");
                    Console.WriteLine("Parte IZQ: \n{0}", parteIzq);
                    Console.WriteLine("Parte DER: \n{0}", parteDer);
                    // Si la Parte IZQ. es una matriz Identidad -> A es invertible
                    // en caso CONTRARIO : A NO es Invertible.
                    if (!(Matrix.IsIdentityMatrix(parteIzq)))
                    {
                        // la parte IZQ. NO es Matriz Identidad
                        // la matriz NO es invertible
                        throw new NotDefinedMatrixOperationException($"La matriz [{this.ToString()}] NO ES INVERTIBLE.");
                    }
                    else
                    {
                        // en la Parte IZQ. queda una Matriz Identidad
                        inversa = parteDer;
                    }
                }
            }
            return inversa;
        }//--fin Inverse()

        //--IdentityMatrix : Implementación OK
        public static Matrix IdentityMatrix(int n)
        {
            // crear matriz Cuadrada n x n
            Matrix identityM = null;
            (int mRows, int nCols) size;
            identityM = new Matrix(n, n);
            size = identityM.GetSize();
            // Garantizar llenado con 0's
            for (int iRow = 0; iRow < size.mRows; iRow++)
            {
                for (int jCol = 0; jCol < size.nCols; jCol++)
                {
                    identityM.SetValueAt(iRow, jCol, 0.0);
                }
            }
            // Setear 1's en Diagonal principal
            // Dado que la matriz es CUADRADA es posible utilizar como límite size.mRows o size.nCols
            for (int i = 0; i < size.mRows; i++)
            {
                // Diagonal principal  i,i
                identityM.SetValueAt(i, i, 1.0);
            }
            return identityM;
        }//fin; IdenityMatrix

        // IsIdentityMatrix  : Implementación Correcta
        public static bool IsIdentityMatrix(Matrix A)
        {
            bool isIdentity = true;
            (int mRows, int nCols) sizeA;
            if (A is null)
            {
                throw new ArgumentNullException("No se puede determinar si una Matriz NULL es Identidad.");
            }
            else
            {
                // Diagonal principal con i = j
                sizeA = A.GetSize();
                for (int iRow = 0; iRow < sizeA.mRows; iRow++)
                {
                    for (int jCol = 0; jCol < sizeA.nCols; jCol++)
                    {
                        // si es Posición de DIAGONAL PRINCIPAL
                        if (iRow == jCol)
                        {
                            // es Posición de DIAGONAL PRINCIPAL
                            // En la Diagonal Principal deben ser valores 1's
                            if ((A.GetValueAt(iRow, jCol)) != 1)
                            {
                                // ERROR: NO es Matriz Identidad
                                isIdentity = false;
                                break;
                            }
                        }
                        else
                        {
                            // si NO ES POSICIÓN de la Diagonal Principal
                            // Valor debe ser 0.0
                            if (A.GetValueAt(iRow, jCol) != 0.0)
                            {
                                // ERROR: NO es Matriz Identidad
                                isIdentity = false;
                                break;
                            }
                        }
                    }
                    //++
                    if (!isIdentity)
                    {
                        break;
                    }
                }// fin recorrido
            }
            return isIdentity;
        }//fin: IsIdentityMatrix
        public override string ToString()
        {
            string s = "";
            s = ArrayUtils.ToBeautyString(_inner2DArray);
            return s;
            //return base.ToString();
        }
    }//--fin: public class Matrix
    //##-- FIN: Vectores y Matrices

    //##++ INI: FilesUtils
    public class PathNotFoundException : Exception
    {
        public PathNotFoundException()
        {
        }

        public PathNotFoundException(string message) : base(message)
        {

        }

        public PathNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    } //-- class PathNotFoundException

    public class WriteToFileException : Exception
    {
        public WriteToFileException()
        {
        }

        public WriteToFileException(string message) : base(message)
        {
        }

        public WriteToFileException(string message, Exception inner) : base(message, inner)
        {
        }
    } //-- class WriteToFileException

    public class FilesUtil
    {

        private static string DefaultStorageFileName = "MatricesFileEECApp.txt";
        public FilesUtil()
        {
        }

        public static string GetDefaultStorageFileName() {
            return DefaultStorageFileName;
        }
        // ++For more info: https://stackoverflow.com/questions/15653921/get-current-folder-path
        // Obtiene la ruta (path) desde la que se esta ejecutando la aplicación (.exe). 
        public string GetCurrentExecutablePath()
        {
            string path = "";
            try
            {
                path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            catch (Exception e)
            {
                throw new PathNotFoundException("No fue posible determinar la RUTA (Path) del ejecutable (.exe)", e);
            }
            return path;
        } //-- GetCurrentExecutablePath

        // Método para agregar la cadena <content> al archivo con el nombre <fileName>.
        // Si el archivo NO existe es creado por el método StreamWriter.WriteLine(),
        // el archivo creado se ubica en el mismo directorio en el que se encuentre el 
        // archivo ejecutable (.exe) 
        // * el método StreamWriter.Writeline() también admite la especificación de rutas
        // absolutas o relativas para la ubicación del archivo.
        // For more info: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-write-to-a-text-file
        private void WriteLine(string fileName, string content)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException("El nombre del archivo NO puede ser null.");
            }
            else if (fileName.Trim().Length == 0)
            {
                throw new ArgumentException("El nombre del archivo NO puede ser una cadena en blanco (vacía).");
            }

            if (content is null)
            {
                throw new ArgumentNullException("La cadena a escribir en el archivo NO puede ser null.");
            }

            try
            {
                using (StreamWriter file = new StreamWriter(fileName, true))
                {
                    file.WriteLine(content);
                }
            }
            catch (Exception e)
            {
                throw new WriteToFileException($"A ocurido un error al intentar escribir la cadena [{content}] en el archivo [{fileName}].", e);
            }
        } //-- WriteLine

        public void WriteLine(string content)
        {
            WriteLine(DefaultStorageFileName, content);
        } //-- public void WriteLine(string content)

        private void ShowFileContents(string fileName)
        {
            string line = "";
            if (fileName is null)
            {
                throw new ArgumentNullException("El nombre del archivo a leer NO puede ser null.");
            }
            else if (fileName.Trim().Length == 0)
            {
                throw new ArgumentException("El nombre del archivo a leer NO puede ser vacío.");
            }

            Console.WriteLine($"##--Contenido del archivo[{fileName}]--##");
            using (StreamReader sr = new StreamReader(fileName))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
            Console.WriteLine($"++FIN del contenido del archivo[{fileName}]++");

        } //-- private void ShowFileContents(string fileName)

        public void ShowFileContents()
        {
            ShowFileContents(DefaultStorageFileName);
        } //-- public void ShowFileContents

        // Valida la existencia del archivo especificado por <path>
        public bool FileExists(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException("La ruta (path) especificada para la validación de la existencia de un archivo NO puede ser null.");
            }
            else if (path.Trim().Length == 0)
            {
                throw new ArgumentException("La ruta (path) especificada para la validación de la existencia de un archivo NO puede ser una cadena vacía.");
            }
            return File.Exists(path);
        } //-- public bool FileExists

        public bool IsEmptyFile(string path)
        {
            FileInfo fInfo;
            long size;
            bool isEmpty;
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"El archivo [{path}] NO existe.");
            }
            else
            {
                fInfo = new FileInfo(path);
                size = fInfo.Length; // tamaño del archivo en bytes
                if (size > 0)
                {
                    // El archivo NO está VACÍO
                    isEmpty = false;
                }
                else
                {
                    // El archivo ESTÁ VACÍO
                    isEmpty = true;
                }
            }
            return isEmpty;
        } //-- public bool IsEmptyFile

        public long FileSize(string path)
        {
            FileInfo fInfo;
            long size;
            if (!File.Exists(path))
            {
                size = -1;
            }
            else
            {
                fInfo = new FileInfo(path);
                size = fInfo.Length; // tamanio en bytes del archivo indicado por <path>
            }
            return size;
        } //-- public long FileSize

    } //-- class FilesUtil
    //##-- FIN: FilesUtils

    class Program
    {
        //**++ Init: Auxiliary main functions
        public static void Case1MatrixAddition() {
            InputReader iReader = new InputReader();
            Matrix matrixA, matrixB, matrixC;
            FilesUtil filesU = new FilesUtil();
            DateTime now = DateTime.Now;
            (int mRows, int nCols) sizeA, sizeB;
            char continueP = 'n';
            uint numRenglA, numRenglB, numColA, numColB;
            do {
                Console.Clear();
                Console.WriteLine("Suma de Matrices.");
                Console.WriteLine("NOTA.- Recuerde que la SUMA de Matrices SÓLO ESTA DEFINIDA PARA MATRICES DEL MISMO TAMAÑO (m x n).");
                numRenglA = iReader.ReadSize("Número de Renglones de la Matriz A: ");
                numColA = iReader.ReadSize("Número de Columnas de la Matriz A: ");

                matrixA = new Matrix((int)numRenglA, (int)numColA);
                matrixB = new Matrix((int)numRenglA, (int)numColA);

                Console.WriteLine("\nDado de la SUMA de matrices SÓLO ESTA DEFINIDA PARA MATRICES DEL MISMO TAMAÑO,");
                Console.WriteLine("las dimensiones de la Matriz B se HAN ESTABLECIDO AUTOMÁTICAMENTE al mismo tamaño");
                Console.WriteLine("definido para la Matriz A.");

                sizeB = matrixB.GetSize();
                Console.WriteLine("\nLas dimesiones de la matriz B son {0}", sizeB);

                //Console.WriteLine("Num. Renglones Matriz A: {0}", numRenglA);
                //Console.WriteLine("Núm. de Columnas Matriz A: {0}", numColA);
                Console.WriteLine("\nIngrese los datos para la Matriz A: ");
                iReader.ReadMatrixData(matrixA);
                Console.WriteLine("\nIngrese los datos para la Matriz B: ");
                iReader.ReadMatrixData(matrixB);

                matrixC = matrixA.AddMatrix(matrixB);

                //##-- Los RESULTADOS DE LA OPERACIÓN SON LOS QUE SE REGISTRAN EN EL ARCHIVO
                Console.WriteLine("##--Resultados de operación--##");
                Console.WriteLine("Matriz A: \n{0}", matrixA);
                Console.WriteLine("Matriz B: \n{0}", matrixB);

                Console.WriteLine("A + B = \n{0}", matrixC);

                try {
                    //--Registrar en Archivo
                    filesU.WriteLine($"Resultado de operación SUMA de Matrices. Registrado [{now}]: \n");
                    filesU.WriteLine($"Matriz A: \n{matrixA}");
                    filesU.WriteLine($"Matriz B: \n{matrixB}");
                    filesU.WriteLine($"\nA + B = \n{matrixC}");
                    filesU.WriteLine("------------------------------------------------------------------");
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
                
                Console.Write("\n\t¿Desea realizar otra suma de Matrices? [y/n]: ");
                continueP = Console.ReadKey().KeyChar;
            } while (Char.ToLower(continueP).Equals('y'));
        }//--fin: Case1MatrixAddition

        public static void Case2RestaMatrices() {
            InputReader iReader = new InputReader();
            Matrix matrixA, matrixB, matrixC;
            (int mRows, int nCols) bSize;
            uint numRenglones, numCols;
            char continueP = 'n';
            do {
                Console.Clear();
                Console.WriteLine("Resta de Matrices.");
                Console.WriteLine("NOTA.- Recuerde que la RESTA de matrices SÓLO ESTA DEFINIDA PARA MATRICES DEL MISMO TAMAÑO.");
                numRenglones = iReader.ReadSize("Ingrese el número de Renglones de la Matriz A: ");
                numCols = iReader.ReadSize("Ingrese el número de Columnas de la Matriz A: ");

                matrixA = new Matrix((int)numRenglones, (int)numCols);
                matrixB = new Matrix((int)numRenglones, (int)numCols);

                Console.WriteLine("Dado que la RESTA de matrices SÓLO ESTÁ DEFINIDA PARA MATRICES de IGUAL TAMAÑO,");
                Console.WriteLine("las dimensiones de la matriz B se HAN CONFIGURADO AUTOMÁTICAMENTE para coincidir con");
                Console.WriteLine("el tamaño especificado para la matriz A.");

                bSize = matrixB.GetSize();
                Console.WriteLine("\nEl tamaño de la matriz B es: {0}", bSize);

                Console.WriteLine("\nIngrese los valores para la Matriz A: ");
                iReader.ReadMatrixData(matrixA);
                Console.WriteLine("\nIngrese los valores para la Matriz B: ");
                iReader.ReadMatrixData(matrixB);

                matrixC = matrixA.RestarMatrix(matrixB);

                //##--Resultados de la operación
                //## Resultados de la operación son los que se almacenan en el archivo (.txt)
                Console.WriteLine("##--Resultados de la operación--##");
                Console.WriteLine("Matriz A: \n{0}", matrixA);
                Console.WriteLine("Matriz B: \n{0}", matrixB);

                Console.WriteLine("A - B = A + (-B) = A + ((-1)*B) = \n{0}", matrixC);

                Console.WriteLine("\n\t¿Desea realizar otra resta de Matrices? [y/n]: ");
                continueP = Console.ReadKey().KeyChar;
            } while (Char.ToLower(continueP).Equals('y'));
        }//--fin: Case2RestaMatrices
        //**-- End: Auxiliary main functions

        public static void Case3MultiplicacionM() {
            InputReader iReader = new InputReader();
            Matrix matrixA, matrixB, matrixC;
            (int mRows, int nCols) sizeA, sizeB;
            char continueP = 'n';
            uint numRenglA, numRenglB, numColsA, numColsB;
            do {
                Console.Clear();
                Console.WriteLine("Multiplicación de Matrices");
                Console.WriteLine("NOTA.-Recuerde de la multiplicación de matrices SÓLO ESTÁ DEFINIDA para 2 matrices A y B,");
                Console.WriteLine("tales que A (m x n) y B (n x p).");

                numRenglA = iReader.ReadSize("Ingrese el número de Renglones de la Matriz A: ");
                numColsA = iReader.ReadSize("Ingrese el número de Columnas de la Matriz A: ");

                // el numCols de A == numRenlg de B
                numRenglB = numColsA;
                Console.WriteLine("Dado que el número de el NÚMERO DE COLUMNAS de la Matriz A DEBE SER IGUAL");
                Console.WriteLine("al NÚMERO DE RENGLONES de la Matriz B, el número de Renglones de la ");
                Console.WriteLine("Matriz B se ha CONFIGURADO AUTOMÁTICAMENTE para adquirir el valor: [{0}]", numRenglB);

                Console.WriteLine();
                numColsB = iReader.ReadSize("Ingrese el número de COLUMNAS de la Matriz B: ");

                matrixA = new Matrix((int)numRenglA, (int)numColsA);
                matrixB = new Matrix((int)numRenglB, (int)numColsB);

                sizeA = matrixA.GetSize();
                sizeB = matrixB.GetSize();

                Console.WriteLine("Tamaño de la matriz A: {0}", sizeA);
                Console.WriteLine("Tamaño de la matriz B: {0}", sizeB);

                Console.WriteLine("Ingrese los DATOS para la Matriz A: ");
                iReader.ReadMatrixData(matrixA);
                Console.WriteLine("Ingrese los DATOS para la Matriz B: ");
                iReader.ReadMatrixData(matrixB);

                matrixC = matrixA.MultiplicacionPorMatriz(matrixB);

                //##--Resultados de la operación
                Console.WriteLine("Matriz A: \n{0}", matrixA);
                Console.WriteLine("Matriz B: \n{0}", matrixB);
                Console.WriteLine("AB = \n{0}", matrixC);

                Console.WriteLine("\n\t¿Desea realizar otra multiplicación de matrices? [y/n]: ");
                continueP = Console.ReadKey().KeyChar;
            } while (Char.ToLower(continueP).Equals('y'));
        }//--fin: public static vois Case3MultiplicacionM

        public static void Case4DivisionM() {
            InputReader iReader = new InputReader();
            Matrix matrixA, matrixB, matrixC, inverseB;
            uint numRenglA, numColsA, tamanioCuadrada;
            (int mRows, int nCols) sizeA, sizeB;
            char continueP = 'n';
            do {
                try {
                    Console.Clear();
                    Console.WriteLine("División de Matrices.");
                    Console.WriteLine("NOTA.- La 'División' de matrices NO EXISTE per se, es decir dadas 2 matrices A y B");
                    Console.WriteLine("la 'operación' A/B NO ESTA DEFINIDA, sin embargo es posible calcular el producto AB^-1,");
                    Console.WriteLine("donde B^-1 representa la INVERSA DE B (la inversa de B debe ser tal que: BB^-1 = I; donde I es una Matriz Identidad de dimensión n.)");

                    Console.WriteLine("\nTome en consideración que la INVERSA SÓLO ESTÁ DEFINIDA PARA MATRICES CUADRADAS (n x n),");
                    Console.WriteLine("considere además QUE NO TODAS las matrices cuadradas SON INVERTIBLES, para mayores detalles consulte: ");
                    Console.WriteLine("Grossman, S.(2008). Álgebra lineal. McGraw Hill: México D.F.");

                    Console.WriteLine();
                    numRenglA = iReader.ReadSize("Ingrese el número de Renglones de la Matriz A: ");
                    numColsA = iReader.ReadSize("Ingrese el núemro de Columnas de la Matriz A: ");

                    Console.WriteLine("\nDado que se intentará calcular la matriz Inversa de B, esta DEBE SER CUADRADA.");
                    tamanioCuadrada = iReader.ReadSize("Ingrese la dimensión de B (el valor ingresado definirá el número de Renglones y Columas de la matriz B): ");

                    matrixA = new Matrix((int)numRenglA, (int)numColsA);
                    matrixB = new Matrix((int)tamanioCuadrada, (int)tamanioCuadrada);

                    sizeA = matrixA.GetSize();
                    sizeB = matrixB.GetSize();

                    Console.WriteLine("Tamaño de la Matriz A: {0}", sizeA);
                    Console.WriteLine("Tamaño de la Matriz B: {0}", sizeB);

                    Console.WriteLine();
                    Console.WriteLine("Ingrese los DATOS de la Matriz A: ");
                    iReader.ReadMatrixData(matrixA);
                    Console.WriteLine("Ingrase los DATOS de la Matriz B: ");
                    iReader.ReadMatrixData(matrixB);

                    Console.WriteLine("Matriz A: \n{0}", matrixA);
                    Console.WriteLine("Matriz B: \n{0}", matrixB);

                    Console.WriteLine("Intentando determinar la INVERSA de B...");
                    inverseB = matrixB.Inverse();

                    Console.WriteLine("B^-1 (Inversa de B): \n{0}", inverseB);

                    Console.WriteLine("Se procede a calcular el PRODUCTO AB^-1...");
                    matrixC = matrixA.MultiplicacionPorMatriz(inverseB);

                    Console.WriteLine("Resultado de la 'DIVISIÓN' (AB^-1): \n{0}", matrixC);

                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
                finally {
                    Console.WriteLine("\n\t¿Desea realizar otra división de matrices? [y/n]: ");
                    continueP = Console.ReadKey().KeyChar;
                }
            } while (Char.ToLower(continueP).Equals('y'));
        }//--fin: Case4DivisonM()

        public static void Case5MostrarContenArchivo() {
            FilesUtil filesU = new FilesUtil();
            try {
                Console.Clear();
                // Validar si el Archivo existe
                if (filesU.FileExists(FilesUtil.GetDefaultStorageFileName()))
                {
                    // validar si está vacío
                    if (filesU.IsEmptyFile(FilesUtil.GetDefaultStorageFileName()))
                    {
                        Console.WriteLine("El archivo {0} está VACÍO.", FilesUtil.GetDefaultStorageFileName());
                    }
                    else {
                        filesU.ShowFileContents();
                    }
                }
                else {
                    Console.WriteLine("El archivo {0} NO existe.", FilesUtil.GetDefaultStorageFileName());
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            finally {
                Console.WriteLine("\n\tPresione cualquier tecla para finalizar...");
                Console.ReadKey();
            }
        }//--fin: MostrarContenArchivo
        static void Main(string[] args)
        {
            InputReader iReader = new InputReader();
            int opt;
            bool delay;
            do {
                delay = false;
                Console.Clear();
                Console.WriteLine("##--Operaciones con matrices (by Erick Escamilla Charco).--##");

                Console.WriteLine("1. Suma de Matrices.");
                Console.WriteLine("2. Resta de Matrices.");
                Console.WriteLine("3. Multiplicación de Matrices.");
                Console.WriteLine("4. División de Matrices.");
                Console.WriteLine("5. Mostrar Datos Almacenado en Archivo");
                Console.WriteLine("0. SALIR");

                opt = iReader.ReadInt("\n\tSeleccione una opción (1,2,3,4,5 o 0): ", "\nEl valor ingresado DEBE SER un número ENTERO.", true);

                switch (opt) {
                    case 1:
                        Case1MatrixAddition();
                        break;
                    case 2:
                        Case2RestaMatrices();
                        break;
                    case 3:
                        Case3MultiplicacionM();
                        break;
                    case 4:
                        Case4DivisionM();
                        break;
                    case 5:
                        Case5MostrarContenArchivo();
                        break;
                    case 0:
                        Console.WriteLine("");
                        break;
                    default:
                        Console.WriteLine("\n\tOpción NO VÁLIDA. Seleccione un valor entero entre 0 y 5.");
                        delay = true;
                        break;
                }//--fin: switch

                if (delay) {
                    Thread.Sleep(1700);
                }
            } while (opt != 0);

            Console.WriteLine("\n\t¡Gracias por utilizar Matrix v1.0, hasta la próxima!");
            Thread.Sleep(1600);
        }//--fin: Método Main().
    }
}
