using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.GraphBuilders {
    public class NameDistanceService : INameDistanceService {
        private readonly ILogger _logger;

        public NameDistanceService(ILogger logger) {
            _logger = logger;
        }

        // T: O(A * B)
        public string FindLargestCommonSubstring(ReadOnlySpan<char> stringA, ReadOnlySpan<char> stringB) {
            var aLength = stringA.Length;
            var bLength = stringB.Length;
            var dynamicMemo = new int[aLength + 1, bLength + 1];

            var length = 0;
            var endIndex = 0;

            for (var x = 0; x <= aLength; x++) {
                for (var y = 0; y <= bLength; y++) {
                    if (x == 0 || y == 0) {
                        dynamicMemo[x, y] = 0;
                    }
                    else if (stringA[x - 1] == stringB[y - 1]) {
                        dynamicMemo[x, y] = dynamicMemo[x - 1, y - 1] + 1;
                        if (dynamicMemo[x, y] > length) {
                            length = dynamicMemo[x, y];
                            endIndex = x - 1;
                        }
                    }
                    else {
                        dynamicMemo[x, y] = 0;
                    }
                }
            }

            return length == 0
                ? string.Empty
                : stringA.Slice(endIndex - length + 1, length).ToString();
        }

        /// <summary>
        /// Computes the Levenshtein distance of two strings using the Wagner-Fischer algorithm
        /// </summary>
        /// <returns>
        /// An integer representing the number of changes, insertions, and/or deletions required to get from <paramref name="stringA"/> to <paramref name="stringB"/>
        /// </returns>
        public int FindDistance(ReadOnlySpan<char> stringA, ReadOnlySpan<char> stringB) {
            var operationNodes = RunWagnerFischer(stringA, stringB);
            return operationNodes[stringA.Length - 1, stringB.Length - 1];
        }

        private ushort[,] RunWagnerFischer(ReadOnlySpan<char> stringA, ReadOnlySpan<char> stringB) {
            if (stringA.Length == 0 || stringB.Length == 0) {
                _logger.Debug($"'{stringA}' or '{stringB}' has a length of 0.");
            }

            var operationNodes = new ushort[stringA.Length, stringB.Length];

            for (var i = 0; i < stringA.Length; i++) {
                var toSet = i == 0 ? (ushort)0 : operationNodes[i - 1, 0];

                if (stringA[i] != stringB[0]) {
                    toSet++;
                }

                operationNodes[i, 0] = toSet;
            }

            for (var i = 1; i < stringB.Length; i++) {
                var toSet = i == 0 ? (ushort)0 : operationNodes[0, i - 1];

                if (stringA[0] != stringB[i]) {
                    toSet++;
                }

                operationNodes[0, i] = toSet;
            }

            for (var x = 1; x < stringA.Length; x++) {
                var letterA = stringA[x];

                for (var y = 1; y < stringB.Length; y++) {
                    var letterB = stringB[y];

                    var sub = operationNodes[x - 1, y - 1];
                    var yOffset = operationNodes[x, y - 1];
                    var xOffset = operationNodes[x - 1, y];

                    var toSet = Math.Min(Math.Min(sub, yOffset), xOffset);

                    if (letterA != letterB) {
                        toSet++;
                    }

                    operationNodes[x, y] = toSet;
                }
            }

            return operationNodes;
        }
    }
}
