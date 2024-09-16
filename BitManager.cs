using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ArthurProduct
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BitManager : UdonSharpBehaviour
    {
        void Start() { }

        // 指定されたポジションから10進数の数値を取得
        public ulong GetDecimalFromPositions(ulong flags, params int[] positions)
        {
            if (flags == 0)
            {
                return 0;
            }
            ulong result = 0;
            foreach (var position in positions)
            {
                CheckPosition(position);
                if (HasFlag(flags, position))
                {
                    for (int i = 0; i < positions.Length; i++)
                    {
                        if (positions[i] == position)
                        {
                            result |= (1UL << i);
                            break;
                        }
                    }
                }
            }
            return result + 1;
        }

        // 10進数の値を指定されたポジションに格納
        public ulong SetPositionsFromDecimal(
            ulong flags,
            ulong decimalValue,
            params int[] positions
        )
        {
            decimalValue--;
            ulong newFlags = flags;
            ulong maxValue = (1UL << positions.Length) - 1;
            if (decimalValue > maxValue)
            {
                Debug.LogError("Decimal value is too large");
            }
            // 清掃する必要があるポジションをクリア
            foreach (var position in positions)
            {
                newFlags = ClearFlag(newFlags, position);
            }

            for (int i = 0; i < positions.Length; i++)
            {
                if ((decimalValue & (1UL << i)) != 0)
                {
                    newFlags = SetFlag(newFlags, positions[i]);
                }
            }
            return newFlags;
        }

        public void CheckPosition(int position)
        {
            if (position < 0 || position >= 64)
            {
                Debug.LogError("Position out of range");
            }
        }

        public ulong SetFlag(ulong flags, int position)
        {
            ulong newFlags = flags;
            CheckPosition(position);
            newFlags |= (1UL << position);
            return newFlags;
        }

        public ulong ClearFlag(ulong flags, int position)
        {
            ulong newFlags = flags;
            CheckPosition(position);
            newFlags &= ~(1UL << position);
            return newFlags;
        }

        public bool HasFlag(ulong flags, int position)
        {
            CheckPosition(position);
            return (flags & (1UL << position)) != 0;
        }
    }
}
