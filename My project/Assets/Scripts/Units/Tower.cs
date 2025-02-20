using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units{
    public class Tower : Unit
    {

        // 禁用移动功能
        protected override void MoveTowardsEnemy()
        {
            // 不执行任何操作
        }

    }
}
