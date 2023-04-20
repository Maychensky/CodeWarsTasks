using System.Numerics;
using System.Text;

namespace CodeWarsTasks
{
    public class Generator
    {
        public int[][] createQrCode(string text)
        {
            StringBuilder bitsSBFormat = new StringBuilder();
            string qrCodeBinaryFormat = GetQrCodeBinaryFormat(ref bitsSBFormat, text);
            var errorCorrectionNumbers = GetErrorCorrectionNumbers(qrCodeBinaryFormat);
            AddErrorCorrectionNumbers(ref bitsSBFormat, errorCorrectionNumbers);
            QRCode qRCode = new QRCode(bitsSBFormat.ToString());
            return qRCode.GetQRCode();
        }

        private void AddErrorCorrectionNumbers(ref StringBuilder bitsSBFormat, List<int> errorCorrectionNumbers)
        {
            foreach (var errorCorrectionNumber in errorCorrectionNumbers)
                bitsSBFormat.Append(GetBinaryFormat(errorCorrectionNumber));
        }

        private string GetQrCodeBinaryFormat(ref StringBuilder bitsSBFormat, string text)
        {
            AddEncodingQrText(ref bitsSBFormat, text);
            AddEncodingQrEnd(ref bitsSBFormat);
            return bitsSBFormat.ToString();
        }

        private void AddEncodingQrText(ref StringBuilder bitsSBFormat, string text)
        {
            bitsSBFormat.Append("0100");
            bitsSBFormat.Append(GetBinaryFormat(text.Length));
            foreach (char letter in text)
                bitsSBFormat.Append(GetBinaryFormat(letter));
            AddEncodingQrEndText(ref bitsSBFormat);
        }

        private void AddEncodingQrEndText(ref StringBuilder bitsSBFormat)
        {
            if (bitsSBFormat.Length < 69) bitsSBFormat.Append("0000");
            else bitsSBFormat.Append(string.Empty.PadLeft(72 - bitsSBFormat.Length, '0'));
        }
        private void AddEncodingQrEnd(ref StringBuilder bitsSBFormat)
        {
            bool isEvenAddLastBytes = true;
            while (bitsSBFormat.Length + 8 <= 72)
            {
                bitsSBFormat.Append((isEvenAddLastBytes) ? "11101100" : "00010001");
                isEvenAddLastBytes = !isEvenAddLastBytes;
            }
        }

        private List<int> GetErrorCorrectionNumbers(string qrCodeBinaryFormat)
        {
            List<int> decimals = new List<int>();
            for (int i = 0; i + 8 <= qrCodeBinaryFormat.Length; i += 8)
                decimals.Add(GetDecimalFormat(qrCodeBinaryFormat.Substring(i, 8)));
            Polynomial polynomial = new Polynomial(decimals);
            var errorCorrectionNumbers = polynomial.GetErrorCorrectionNumbers();
            while(errorCorrectionNumbers.Count < 17) errorCorrectionNumbers.Insert(0, 0);
            return errorCorrectionNumbers;
        }

        private string GetBinaryFormat(int decimalNumber, int numberRequiredZeros = 8) => Convert.ToString(decimalNumber, 2).PadLeft(numberRequiredZeros, '0');
        private int GetDecimalFormat(string binaryValue) => Convert.ToInt32(binaryValue, 2);
    }

    public class Polynomial
    {
        private List<MemberPolynomial> _generatorPolynomial;
        private List<MemberPolynomial> _massagePolynomial;
        static Polynomial()
        {
            
        }
        public Polynomial(List<int> massagePolynomial)
        {
            _massagePolynomial = new List<MemberPolynomial>();
            int degreeVariable = massagePolynomial.Count;
            foreach (var elementMassagePolynomial in massagePolynomial)
                _massagePolynomial.Add(new (--degreeVariable, elementMassagePolynomial, MemberPolynomial.MultiplierState.NumericalMultiplier));
            _generatorPolynomial = new List<MemberPolynomial>
            {
                new MemberPolynomial( 25, 0, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 24, 43, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 23, 139, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 22, 206, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 21, 78, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 20, 43, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 19, 239, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 18, 123, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 17, 206, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 16, 214, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 15, 147, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 14, 24, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 13, 99, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 12, 150, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 11, 39, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 10, 243, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 9, 163, MemberPolynomial.MultiplierState.VariableWithDegree),
                new MemberPolynomial( 8, 136, MemberPolynomial.MultiplierState.VariableWithDegree),
            };
        }

        public List<int> GetErrorCorrectionNumbers()
        {
            MultiplyByVariableInDegree(17, _massagePolynomial);
            int countIteration = 0;
            while (countIteration < 9) countIteration += IterationDeleteFirstElementPolynomial();
            return GetCoefficients();
        }

        private List<int> GetCoefficients()
        {
            List<int> result = new List<int>();
            foreach (var e in _massagePolynomial)
                result.Add(e.numericalMultiplier);
            return result;
        }

        private int IterationDeleteFirstElementPolynomial()
        {
            ResetPolynomials(_generatorPolynomial);
            MultiplyByMultiplierVariableInDegree(_massagePolynomial.First().multiplierVariableWithDegree, _generatorPolynomial);
            XORForPolynomials(_massagePolynomial, _generatorPolynomial);
            return DeleteFirstZeroMembers();
        }

        private int DeleteFirstZeroMembers()
        {
            int numberFirstElementsZero = 0;
            for (int i = 0; i < _massagePolynomial.Count; i++)
                if (_massagePolynomial[i].IsZero()) numberFirstElementsZero++;
                else break;
            _massagePolynomial.RemoveRange(0, numberFirstElementsZero);
            return numberFirstElementsZero;
        }

        public void MultiplyByVariableInDegree(int degree, List<MemberPolynomial> changePolynomial)
        {
            foreach (var elementMassagePolynomial in changePolynomial)
                elementMassagePolynomial.MultiplyByVariableInDegree(degree);
        }

        public void MultiplyByMultiplierVariableInDegree(int degree, List<MemberPolynomial> changePolynomial)
        {
            foreach (var elementMassagePolynomial in changePolynomial)
                elementMassagePolynomial.MultiplyByMultiplierVariableInDegree(degree);
        }

        public void XORForPolynomials(List<MemberPolynomial> leftPolynomial, List<MemberPolynomial> rightPolynomial)
        {
            if (leftPolynomial.Count > rightPolynomial.Count) throw new Exception("левый больше правого");
            for(int i = 0; i < rightPolynomial.Count; i++)
            {
                if (i < leftPolynomial.Count) 
                    leftPolynomial[i].XORByNumericalMultiplier(rightPolynomial[i]);
                else
                {
                    leftPolynomial.Add(new ());
                    leftPolynomial[i].XORByNumericalMultiplier(rightPolynomial[i]);
                }
            }
        }

        private void ResetPolynomials(List<MemberPolynomial> changePolynomials)
        {
            foreach (var memberPolynomials in changePolynomials)
                memberPolynomials.ResetMultiplier();
        }
    }

    public class MemberPolynomial
    {
        private static readonly Dictionary<int, int> _preloadedGetDegree = new Dictionary<int, int>
        {
            {   1   ,   0   }   ,
            {   2   ,   1   }   ,
            {   3   ,   25  }   ,
            {   4   ,   2   }   ,
            {   5   ,   50  }   ,
            {   6   ,   26  }   ,
            {   7   ,   198 }   ,
            {   8   ,   3   }   ,
            {   9   ,   223 }   ,
            {   10  ,   51  }   ,
            {   11  ,   238 }   ,
            {   12  ,   27  }   ,
            {   13  ,   104 }   ,
            {   14  ,   199 }   ,
            {   15  ,   75  }   ,
            {   16  ,   4   }   ,
            {   17  ,   100 }   ,
            {   18  ,   224 }   ,
            {   19  ,   14  }   ,
            {   20  ,   52  }   ,
            {   21  ,   141 }   ,
            {   22  ,   239 }   ,
            {   23  ,   129 }   ,
            {   24  ,   28  }   ,
            {   25  ,   193 }   ,
            {   26  ,   105 }   ,
            {   27  ,   248 }   ,
            {   28  ,   200 }   ,
            {   29  ,   8   }   ,
            {   30  ,   76  }   ,
            {   31  ,   113 }   ,
            {   32  ,   5   }   ,
            {   33  ,   138 }   ,
            {   34  ,   101 }   ,
            {   35  ,   47  }   ,
            {   36  ,   225 }   ,
            {   37  ,   36  }   ,
            {   38  ,   15  }   ,
            {   39  ,   33  }   ,
            {   40  ,   53  }   ,
            {   41  ,   147 }   ,
            {   42  ,   142 }   ,
            {   43  ,   218 }   ,
            {   44  ,   240 }   ,
            {   45  ,   18  }   ,
            {   46  ,   130 }   ,
            {   47  ,   69  }   ,
            {   48  ,   29  }   ,
            {   49  ,   181 }   ,
            {   50  ,   194 }   ,
            {   51  ,   125 }   ,
            {   52  ,   106 }   ,
            {   53  ,   39  }   ,
            {   54  ,   249 }   ,
            {   55  ,   185 }   ,
            {   56  ,   201 }   ,
            {   57  ,   154 }   ,
            {   58  ,   9   }   ,
            {   59  ,   120 }   ,
            {   60  ,   77  }   ,
            {   61  ,   228 }   ,
            {   62  ,   114 }   ,
            {   63  ,   166 }   ,
            {   64  ,   6   }   ,
            {   65  ,   191 }   ,
            {   66  ,   139 }   ,
            {   67  ,   98  }   ,
            {   68  ,   102 }   ,
            {   69  ,   221 }   ,
            {   70  ,   48  }   ,
            {   71  ,   253 }   ,
            {   72  ,   226 }   ,
            {   73  ,   152 }   ,
            {   74  ,   37  }   ,
            {   75  ,   179 }   ,
            {   76  ,   16  }   ,
            {   77  ,   145 }   ,
            {   78  ,   34  }   ,
            {   79  ,   136 }   ,
            {   80  ,   54  }   ,
            {   81  ,   208 }   ,
            {   82  ,   148 }   ,
            {   83  ,   206 }   ,
            {   84  ,   143 }   ,
            {   85  ,   150 }   ,
            {   86  ,   219 }   ,
            {   87  ,   189 }   ,
            {   88  ,   241 }   ,
            {   89  ,   210 }   ,
            {   90  ,   19  }   ,
            {   91  ,   92  }   ,
            {   92  ,   131 }   ,
            {   93  ,   56  }   ,
            {   94  ,   70  }   ,
            {   95  ,   64  }   ,
            {   96  ,   30  }   ,
            {   97  ,   66  }   ,
            {   98  ,   182 }   ,
            {   99  ,   163 }   ,
            {   100 ,   195 }   ,
            {   101 ,   72  }   ,
            {   102 ,   126 }   ,
            {   103 ,   110 }   ,
            {   104 ,   107 }   ,
            {   105 ,   58  }   ,
            {   106 ,   40  }   ,
            {   107 ,   84  }   ,
            {   108 ,   250 }   ,
            {   109 ,   133 }   ,
            {   110 ,   186 }   ,
            {   111 ,   61  }   ,
            {   112 ,   202 }   ,
            {   113 ,   94  }   ,
            {   114 ,   155 }   ,
            {   115 ,   159 }   ,
            {   116 ,   10  }   ,
            {   117 ,   21  }   ,
            {   118 ,   121 }   ,
            {   119 ,   43  }   ,
            {   120 ,   78  }   ,
            {   121 ,   212 }   ,
            {   122 ,   229 }   ,
            {   123 ,   172 }   ,
            {   124 ,   115 }   ,
            {   125 ,   243 }   ,
            {   126 ,   167 }   ,
            {   127 ,   87  }   ,
            {   128 ,   7   }   ,
            {   129 ,   112 }   ,
            {   130 ,   192 }   ,
            {   131 ,   247 }   ,
            {   132 ,   140 }   ,
            {   133 ,   128 }   ,
            {   134 ,   99  }   ,
            {   135 ,   13  }   ,
            {   136 ,   103 }   ,
            {   137 ,   74  }   ,
            {   138 ,   222 }   ,
            {   139 ,   237 }   ,
            {   140 ,   49  }   ,
            {   141 ,   197 }   ,
            {   142 ,   254 }   ,
            {   143 ,   24  }   ,
            {   144 ,   227 }   ,
            {   145 ,   165 }   ,
            {   146 ,   153 }   ,
            {   147 ,   119 }   ,
            {   148 ,   38  }   ,
            {   149 ,   184 }   ,
            {   150 ,   180 }   ,
            {   151 ,   124 }   ,
            {   152 ,   17  }   ,
            {   153 ,   68  }   ,
            {   154 ,   146 }   ,
            {   155 ,   217 }   ,
            {   156 ,   35  }   ,
            {   157 ,   32  }   ,
            {   158 ,   137 }   ,
            {   159 ,   46  }   ,
            {   160 ,   55  }   ,
            {   161 ,   63  }   ,
            {   162 ,   209 }   ,
            {   163 ,   91  }   ,
            {   164 ,   149 }   ,
            {   165 ,   188 }   ,
            {   166 ,   207 }   ,
            {   167 ,   205 }   ,
            {   168 ,   144 }   ,
            {   169 ,   135 }   ,
            {   170 ,   151 }   ,
            {   171 ,   178 }   ,
            {   172 ,   220 }   ,
            {   173 ,   252 }   ,
            {   174 ,   190 }   ,
            {   175 ,   97  }   ,
            {   176 ,   242 }   ,
            {   177 ,   86  }   ,
            {   178 ,   211 }   ,
            {   179 ,   171 }   ,
            {   180 ,   20  }   ,
            {   181 ,   42  }   ,
            {   182 ,   93  }   ,
            {   183 ,   158 }   ,
            {   184 ,   132 }   ,
            {   185 ,   60  }   ,
            {   186 ,   57  }   ,
            {   187 ,   83  }   ,
            {   188 ,   71  }   ,
            {   189 ,   109 }   ,
            {   190 ,   65  }   ,
            {   191 ,   162 }   ,
            {   192 ,   31  }   ,
            {   193 ,   45  }   ,
            {   194 ,   67  }   ,
            {   195 ,   216 }   ,
            {   196 ,   183 }   ,
            {   197 ,   123 }   ,
            {   198 ,   164 }   ,
            {   199 ,   118 }   ,
            {   200 ,   196 }   ,
            {   201 ,   23  }   ,
            {   202 ,   73  }   ,
            {   203 ,   236 }   ,
            {   204 ,   127 }   ,
            {   205 ,   12  }   ,
            {   206 ,   111 }   ,
            {   207 ,   246 }   ,
            {   208 ,   108 }   ,
            {   209 ,   161 }   ,
            {   210 ,   59  }   ,
            {   211 ,   82  }   ,
            {   212 ,   41  }   ,
            {   213 ,   157 }   ,
            {   214 ,   85  }   ,
            {   215 ,   170 }   ,
            {   216 ,   251 }   ,
            {   217 ,   96  }   ,
            {   218 ,   134 }   ,
            {   219 ,   177 }   ,
            {   220 ,   187 }   ,
            {   221 ,   204 }   ,
            {   222 ,   62  }   ,
            {   223 ,   90  }   ,
            {   224 ,   203 }   ,
            {   225 ,   89  }   ,
            {   226 ,   95  }   ,
            {   227 ,   176 }   ,
            {   228 ,   156 }   ,
            {   229 ,   169 }   ,
            {   230 ,   160 }   ,
            {   231 ,   81  }   ,
            {   232 ,   11  }   ,
            {   233 ,   245 }   ,
            {   234 ,   22  }   ,
            {   235 ,   235 }   ,
            {   236 ,   122 }   ,
            {   237 ,   117 }   ,
            {   238 ,   44  }   ,
            {   239 ,   215 }   ,
            {   240 ,   79  }   ,
            {   241 ,   174 }   ,
            {   242 ,   213 }   ,
            {   243 ,   233 }   ,
            {   244 ,   230 }   ,
            {   245 ,   231 }   ,
            {   246 ,   173 }   ,
            {   247 ,   232 }   ,
            {   248 ,   116 }   ,
            {   249 ,   214 }   ,
            {   250 ,   244 }   ,
            {   251 ,   234 }   ,
            {   252 ,   168 }   ,
            {   253 ,   80  }   ,
            {   254 ,   88  }   ,
            {   255 ,   175 }   ,
        };
        private static readonly Dictionary<int, int> _preloadedGetNumerical = new Dictionary<int, int>
        {
            {   0   ,   1   }   ,
            {   1   ,   2   }   ,
            {   2   ,   4   }   ,
            {   3   ,   8   }   ,
            {   4   ,   16  }   ,
            {   5   ,   32  }   ,
            {   6   ,   64  }   ,
            {   7   ,   128 }   ,
            {   8   ,   29  }   ,
            {   9   ,   58  }   ,
            {   10  ,   116 }   ,
            {   11  ,   232 }   ,
            {   12  ,   205 }   ,
            {   13  ,   135 }   ,
            {   14  ,   19  }   ,
            {   15  ,   38  }   ,
            {   16  ,   76  }   ,
            {   17  ,   152 }   ,
            {   18  ,   45  }   ,
            {   19  ,   90  }   ,
            {   20  ,   180 }   ,
            {   21  ,   117 }   ,
            {   22  ,   234 }   ,
            {   23  ,   201 }   ,
            {   24  ,   143 }   ,
            {   25  ,   3   }   ,
            {   26  ,   6   }   ,
            {   27  ,   12  }   ,
            {   28  ,   24  }   ,
            {   29  ,   48  }   ,
            {   30  ,   96  }   ,
            {   31  ,   192 }   ,
            {   32  ,   157 }   ,
            {   33  ,   39  }   ,
            {   34  ,   78  }   ,
            {   35  ,   156 }   ,
            {   36  ,   37  }   ,
            {   37  ,   74  }   ,
            {   38  ,   148 }   ,
            {   39  ,   53  }   ,
            {   40  ,   106 }   ,
            {   41  ,   212 }   ,
            {   42  ,   181 }   ,
            {   43  ,   119 }   ,
            {   44  ,   238 }   ,
            {   45  ,   193 }   ,
            {   46  ,   159 }   ,
            {   47  ,   35  }   ,
            {   48  ,   70  }   ,
            {   49  ,   140 }   ,
            {   50  ,   5   }   ,
            {   51  ,   10  }   ,
            {   52  ,   20  }   ,
            {   53  ,   40  }   ,
            {   54  ,   80  }   ,
            {   55  ,   160 }   ,
            {   56  ,   93  }   ,
            {   57  ,   186 }   ,
            {   58  ,   105 }   ,
            {   59  ,   210 }   ,
            {   60  ,   185 }   ,
            {   61  ,   111 }   ,
            {   62  ,   222 }   ,
            {   63  ,   161 }   ,
            {   64  ,   95  }   ,
            {   65  ,   190 }   ,
            {   66  ,   97  }   ,
            {   67  ,   194 }   ,
            {   68  ,   153 }   ,
            {   69  ,   47  }   ,
            {   70  ,   94  }   ,
            {   71  ,   188 }   ,
            {   72  ,   101 }   ,
            {   73  ,   202 }   ,
            {   74  ,   137 }   ,
            {   75  ,   15  }   ,
            {   76  ,   30  }   ,
            {   77  ,   60  }   ,
            {   78  ,   120 }   ,
            {   79  ,   240 }   ,
            {   80  ,   253 }   ,
            {   81  ,   231 }   ,
            {   82  ,   211 }   ,
            {   83  ,   187 }   ,
            {   84  ,   107 }   ,
            {   85  ,   214 }   ,
            {   86  ,   177 }   ,
            {   87  ,   127 }   ,
            {   88  ,   254 }   ,
            {   89  ,   225 }   ,
            {   90  ,   223 }   ,
            {   91  ,   163 }   ,
            {   92  ,   91  }   ,
            {   93  ,   182 }   ,
            {   94  ,   113 }   ,
            {   95  ,   226 }   ,
            {   96  ,   217 }   ,
            {   97  ,   175 }   ,
            {   98  ,   67  }   ,
            {   99  ,   134 }   ,
            {   100 ,   17  }   ,
            {   101 ,   34  }   ,
            {   102 ,   68  }   ,
            {   103 ,   136 }   ,
            {   104 ,   13  }   ,
            {   105 ,   26  }   ,
            {   106 ,   52  }   ,
            {   107 ,   104 }   ,
            {   108 ,   208 }   ,
            {   109 ,   189 }   ,
            {   110 ,   103 }   ,
            {   111 ,   206 }   ,
            {   112 ,   129 }   ,
            {   113 ,   31  }   ,
            {   114 ,   62  }   ,
            {   115 ,   124 }   ,
            {   116 ,   248 }   ,
            {   117 ,   237 }   ,
            {   118 ,   199 }   ,
            {   119 ,   147 }   ,
            {   120 ,   59  }   ,
            {   121 ,   118 }   ,
            {   122 ,   236 }   ,
            {   123 ,   197 }   ,
            {   124 ,   151 }   ,
            {   125 ,   51  }   ,
            {   126 ,   102 }   ,
            {   127 ,   204 }   ,
            {   128 ,   133 }   ,
            {   129 ,   23  }   ,
            {   130 ,   46  }   ,
            {   131 ,   92  }   ,
            {   132 ,   184 }   ,
            {   133 ,   109 }   ,
            {   134 ,   218 }   ,
            {   135 ,   169 }   ,
            {   136 ,   79  }   ,
            {   137 ,   158 }   ,
            {   138 ,   33  }   ,
            {   139 ,   66  }   ,
            {   140 ,   132 }   ,
            {   141 ,   21  }   ,
            {   142 ,   42  }   ,
            {   143 ,   84  }   ,
            {   144 ,   168 }   ,
            {   145 ,   77  }   ,
            {   146 ,   154 }   ,
            {   147 ,   41  }   ,
            {   148 ,   82  }   ,
            {   149 ,   164 }   ,
            {   150 ,   85  }   ,
            {   151 ,   170 }   ,
            {   152 ,   73  }   ,
            {   153 ,   146 }   ,
            {   154 ,   57  }   ,
            {   155 ,   114 }   ,
            {   156 ,   228 }   ,
            {   157 ,   213 }   ,
            {   158 ,   183 }   ,
            {   159 ,   115 }   ,
            {   160 ,   230 }   ,
            {   161 ,   209 }   ,
            {   162 ,   191 }   ,
            {   163 ,   99  }   ,
            {   164 ,   198 }   ,
            {   165 ,   145 }   ,
            {   166 ,   63  }   ,
            {   167 ,   126 }   ,
            {   168 ,   252 }   ,
            {   169 ,   229 }   ,
            {   170 ,   215 }   ,
            {   171 ,   179 }   ,
            {   172 ,   123 }   ,
            {   173 ,   246 }   ,
            {   174 ,   241 }   ,
            {   175 ,   255 }   ,
            {   176 ,   227 }   ,
            {   177 ,   219 }   ,
            {   178 ,   171 }   ,
            {   179 ,   75  }   ,
            {   180 ,   150 }   ,
            {   181 ,   49  }   ,
            {   182 ,   98  }   ,
            {   183 ,   196 }   ,
            {   184 ,   149 }   ,
            {   185 ,   55  }   ,
            {   186 ,   110 }   ,
            {   187 ,   220 }   ,
            {   188 ,   165 }   ,
            {   189 ,   87  }   ,
            {   190 ,   174 }   ,
            {   191 ,   65  }   ,
            {   192 ,   130 }   ,
            {   193 ,   25  }   ,
            {   194 ,   50  }   ,
            {   195 ,   100 }   ,
            {   196 ,   200 }   ,
            {   197 ,   141 }   ,
            {   198 ,   7   }   ,
            {   199 ,   14  }   ,
            {   200 ,   28  }   ,
            {   201 ,   56  }   ,
            {   202 ,   112 }   ,
            {   203 ,   224 }   ,
            {   204 ,   221 }   ,
            {   205 ,   167 }   ,
            {   206 ,   83  }   ,
            {   207 ,   166 }   ,
            {   208 ,   81  }   ,
            {   209 ,   162 }   ,
            {   210 ,   89  }   ,
            {   211 ,   178 }   ,
            {   212 ,   121 }   ,
            {   213 ,   242 }   ,
            {   214 ,   249 }   ,
            {   215 ,   239 }   ,
            {   216 ,   195 }   ,
            {   217 ,   155 }   ,
            {   218 ,   43  }   ,
            {   219 ,   86  }   ,
            {   220 ,   172 }   ,
            {   221 ,   69  }   ,
            {   222 ,   138 }   ,
            {   223 ,   9   }   ,
            {   224 ,   18  }   ,
            {   225 ,   36  }   ,
            {   226 ,   72  }   ,
            {   227 ,   144 }   ,
            {   228 ,   61  }   ,
            {   229 ,   122 }   ,
            {   230 ,   244 }   ,
            {   231 ,   245 }   ,
            {   232 ,   247 }   ,
            {   233 ,   243 }   ,
            {   234 ,   251 }   ,
            {   235 ,   235 }   ,
            {   236 ,   203 }   ,
            {   237 ,   139 }   ,
            {   238 ,   11  }   ,
            {   239 ,   22  }   ,
            {   240 ,   44  }   ,
            {   241 ,   88  }   ,
            {   242 ,   176 }   ,
            {   243 ,   125 }   ,
            {   244 ,   250 }   ,
            {   245 ,   233 }   ,
            {   246 ,   207 }   ,
            {   247 ,   131 }   ,
            {   248 ,   27  }   ,
            {   249 ,   54  }   ,
            {   250 ,   108 }   ,
            {   251 ,   216 }   ,
            {   252 ,   173 }   ,
            {   253 ,   71  }   ,
            {   254 ,   142 }   ,
            {   255 ,   1   }   ,
        };
        private readonly int _startValueMultiplier;
        private readonly MultiplierState _startValueMultiplierState;
        public enum MultiplierState
        {
            VariableWithDegree,
            NumericalMultiplier,
        }
        public int degreeVariable { get; private set; } 
        private int _numericalMultiplier;
        public int numericalMultiplier 
        { 
            get { return _numericalMultiplier; }  
            private set 
            { 
                _numericalMultiplier = value;
                UpdateNumericalMultiplier(); 
            }  
        }

        private int _multiplierVariableWithDegree;
        public int multiplierVariableWithDegree  
        { 
            get { return _multiplierVariableWithDegree; } 
            private set 
            { 
                _multiplierVariableWithDegree = value;
                UpdateMultiplierVariableWithDegree();
            }
        }

        public MemberPolynomial() : this(0, 0, MultiplierState.NumericalMultiplier) { }
        public MemberPolynomial(int degreeVariable, int multiplier, MultiplierState multiplierState)
        {
            _startValueMultiplier = multiplier;
            _startValueMultiplierState = multiplierState;
            this.degreeVariable = degreeVariable;
            _multiplierVariableWithDegree = 0;
            _numericalMultiplier = 0;
            InitMultiplier(multiplier, multiplierState);
        }
        private void UpdateNumericalMultiplier()
        {
            if (_numericalMultiplier == 0)
            {
                _multiplierVariableWithDegree = 0;
                return;
            }
            _multiplierVariableWithDegree = _preloadedGetDegree[(_numericalMultiplier > 255) ? _numericalMultiplier % 255 : _numericalMultiplier];
        }

        private void UpdateMultiplierVariableWithDegree()
        {
            _numericalMultiplier = _preloadedGetNumerical[(_multiplierVariableWithDegree > 255) ? _numericalMultiplier % 255 : _multiplierVariableWithDegree];
        }

        private void InitMultiplier(int multiplier, MultiplierState multiplierState)
        {
            if (multiplierState.Equals(MultiplierState.NumericalMultiplier)) numericalMultiplier = multiplier;
            else if (multiplierState.Equals(MultiplierState.VariableWithDegree)) multiplierVariableWithDegree = multiplier;
        }

        public void MultiplyByVariableInDegree(int degree) => degreeVariable += degree;
        public void MultiplyByMultiplierVariableInDegree(int degree)
        {
            if (multiplierVariableWithDegree + degree > 255)
            {
                multiplierVariableWithDegree = (multiplierVariableWithDegree + degree) % 255;
                return;
            }
            multiplierVariableWithDegree += degree;
        } 
        public void XORByNumericalMultiplier(MemberPolynomial otherMemberPolynomial) => numericalMultiplier ^= otherMemberPolynomial.numericalMultiplier;
        public bool IsZero() => _multiplierVariableWithDegree == 0 && _numericalMultiplier == 0;
        public void ResetMultiplier() => InitMultiplier(_startValueMultiplier, _startValueMultiplierState);
    }

    public class QRCode
    {
        private const int QRCODE_SIDE_SIZE = 21;
        private int[][] _QRCodeArrayFormat;
        private string _bits;
        private static readonly Dictionary<DirectionVertical, int> _directionValue;
        private enum DirectionVertical { Up, Down, }

        static QRCode()
        {
            _directionValue = new Dictionary<DirectionVertical, int>
            {
                {DirectionVertical.Up, -1},
                {DirectionVertical.Down, 1},
            };
        }

        public QRCode(string bits)
        {
            _bits = bits;
            _QRCodeArrayFormat = new int[][]
            {
                new int[]{1,1,1,1,1,1,1,0,1,0,0,0,0,0,1,1,1,1,1,1,1},
                new int[]{1,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,1},
                new int[]{1,0,1,1,1,0,1,0,0,0,0,0,0,0,1,0,1,1,1,0,1},
                new int[]{1,0,1,1,1,0,1,0,1,0,0,0,0,0,1,0,1,1,1,0,1},
                new int[]{1,0,1,1,1,0,1,0,0,0,0,0,0,0,1,0,1,1,1,0,1},
                new int[]{1,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,1},
                new int[]{1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1},
                new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                new int[]{0,0,1,0,1,1,1,0,1,0,0,0,0,1,0,0,0,1,0,0,1},
                new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                new int[]{0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                new int[]{0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                new int[]{0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                new int[]{1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                new int[]{1,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                new int[]{1,0,1,1,1,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                new int[]{1,0,1,1,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                new int[]{1,0,1,1,1,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                new int[]{1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                new int[]{1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };
        }

        public int[][] GetQRCode() => GetQRCode(_bits);
        public int[][] GetQRCode(string bits)
        {
            // Init right block
            int currentIndexBits = FillVertical(DirectionVertical.Up, new Vector2(20, 20), 12, 0);
            currentIndexBits = FillVertical(DirectionVertical.Down, new Vector2(18, 9), 12, currentIndexBits);
            currentIndexBits = FillVertical(DirectionVertical.Up, new Vector2(16, 20), 12, currentIndexBits);
            currentIndexBits = FillVertical(DirectionVertical.Down, new Vector2(14, 9), 12, currentIndexBits);
            // Init middle block
            currentIndexBits = FillVertical(DirectionVertical.Up, new Vector2(12, 20), 14, currentIndexBits);
            currentIndexBits = FillVertical(DirectionVertical.Up, new Vector2(12, 5), 6, currentIndexBits);
            currentIndexBits = FillVertical(DirectionVertical.Down, new Vector2(10, 0), 6, currentIndexBits);
            currentIndexBits = FillVertical(DirectionVertical.Down, new Vector2(10, 7), 14, currentIndexBits);
            // Init left block
            currentIndexBits = FillVertical(DirectionVertical.Up, new Vector2(8, 12), 4, currentIndexBits);
            currentIndexBits = FillVertical(DirectionVertical.Down, new Vector2(5, 9), 4, currentIndexBits);
            currentIndexBits = FillVertical(DirectionVertical.Up, new Vector2(3, 12), 4, currentIndexBits);
            FillVertical(DirectionVertical.Down, new Vector2(1, 9), 4, currentIndexBits);
            return _QRCodeArrayFormat;
        }
        private int FillVertical(DirectionVertical direction, Vector2 startPosition, int sizeVertical, int startIndexBits)
        {
            int directionValue = _directionValue[direction];
            int startPositionX = (int)startPosition.X;
            int startPositionY = (int)startPosition.Y;
            int endPositionY = startPositionY + (directionValue * sizeVertical);
            for (int y = startPositionY; y != endPositionY; y += directionValue)
            {
                InitElementWithMask(startPositionX, y, startIndexBits++);
                InitElementWithMask(startPositionX - 1, y, startIndexBits++);
            }
            return startIndexBits;
        }

        private void InitElementWithMask(int x, int y, int indexBits)
        {
            int bit = (int)char.GetNumericValue(_bits[indexBits]);
            _QRCodeArrayFormat[y][x] = ((x + y) % 2 == 0) ? ReversBits(bit) : bit;
        }

        private int ReversBits(int value) => (value == 0) ? 1 : 0;
    }

}
