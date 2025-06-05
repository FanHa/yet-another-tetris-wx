// using System;
// using System.Collections.Generic;
// using System.Linq;
// using TMPro;
// using UnityEngine;

// namespace Units.Skills
// {
//     [AddComponentMenu("Units/Skills/Manager")]
//     public class Manager : MonoBehaviour
//     {
//         public float CooldownRevisePercentage;
//         private List<Skill> skills = new List<Skill>();
//         private Unit owner;
//         public int SkillsCount => skills.Count;

//         private Skill readySkill;

//         public event Action<Unit, Skill> OnSkillCast;

//         public float energyPerTick = 10f;

//         private float tickTimer = 0f;
//         private const float TICK_INTERVAL = 0.5f;
//         private float energyDecayPerSkill = 0.8f; // 每多一个技能，能量再乘以这个系数
//         private bool isActive = false;

//         void Awake()
//         {
//             owner = GetComponent<Unit>();
//             if (owner == null)
//             {
//                 Debug.LogError("Unit component not found on the same GameObject as SkillsManager.");
//             }
//         }

//         void Update()
//         {
//             if (!isActive || skills.Count == 0)
//             {
//                 return; // 如果没有技能或未激活，直接返回
//             }
//             tickTimer += Time.deltaTime;
//             if (tickTimer >= TICK_INTERVAL)
//             {
//                 tickTimer -= TICK_INTERVAL;
//                 Tick();
//             }
//         }

//         private void Tick()
//         {
//             int skillCount = skills.Count;
//             if (skillCount == 0) return;

//             // 计算衰减后的能量比例
//             float decay = Mathf.Pow(energyDecayPerSkill, skillCount - 1);
//             float gain = energyPerTick * decay;

//             foreach (var skill in skills)
//             {
//                 skill.AddEnergy(gain);
//             }

//             readySkill = skills.FirstOrDefault(skill => skill.IsReady());
//         }

//         public void Activate()
//         {
//             isActive = true;
//             tickTimer = 0f;
//         }

//         public void Deactivate()
//         {
//             isActive = false;
//         }

//         public void AddSkill(Skill newSkill)
//         {
//             // 检查是否已经存在相同类型的技能
//             if (skills.Any(skill => skill.GetType() == newSkill.GetType()))
//             {
//                 return;
//             }

//             skills.Add(newSkill);
//         }

//         public void CastSkill()
//         {
//             if (readySkill != null)
//             {
//                 OnSkillCast?.Invoke(owner, readySkill);
//                 readySkill.Execute(owner); // 执行保存的技能
//                 readySkill = null; // 重置就绪技能
//             }
//         }

//         public bool HasSkillToTrigger()
//         {
//             if (readySkill != null)
//             {
//                 return false;
//             }
//             if (skills.Count <= 0)
//             {
//                 return false;
//             }
//             readySkill = skills.FirstOrDefault(skill => skill.IsReady()); // 找到第一个就绪的技能
//             return readySkill != null; // 如果找到就绪技能，返回 true

//         }
//     }
// }