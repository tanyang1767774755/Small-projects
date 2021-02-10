using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using xna = Microsoft.Xna.Framework;
using URWPGSim2D.Common;
using URWPGSim2D.StrategyLoader;

namespace URWPGSim2D.Strategy
{
    public class Strategy : MarshalByRefObject, IStrategy
    {
        public static float xzdangle, F1xzdangle;
        #region reserved code never be changed or removed
        /// <summary>
        /// override the InitializeLifetimeService to return null instead of a valid ILease implementation
        /// to ensure this type of remote object never dies
        /// </summary>
        /// <returns>null</returns>
        public override object InitializeLifetimeService()
        {

            //return base.InitializeLifetimeService();
            return null; // makes the object live indefinitely
        }
        #endregion

        /// <summary> 
        /// 决策类当前对象对应的仿真使命参与队伍的决策数组引用 第一次调用GetDecision时分配空间
        /// </summary>
        private Decision[] decisions = null;

        /// <summary>
        /// 获取队伍名称 在此处设置参赛队伍的名称
        /// </summary>
        /// <returns>队伍名称字符串</returns>
        public string GetTeamName()
        {
            return "西南 ~0~ 民大";
        }


        void OutFile(String Path, String context)
        {

            if (!File.Exists(Path))
                File.Create(Path);
            using (FileStream fs = File.Open(Path, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(context);
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }
        }


        #region    返回两点间距离(通过坐标获得距离)
        /// <summary>
        /// 返回两点间距离
        /// </summary>
        /// <param name="cur_x">当前点X坐标</param>
        /// <param name="cur_z">当前点Z坐标</param>
        /// <param name="dest_x">目标点X坐标</param>
        /// <param name="dest_z">目标点Y坐标</param>
        /// <returns></returns>
        float GetLengthToDestpoint(float cur_x, float cur_z, float dest_x, float dest_z)
        {
            return (float)Math.Sqrt(Math.Pow((cur_x - dest_x), 2) + Math.Pow((cur_z - dest_z), 2));
        }
        #endregion


        #region  根据需要返回一个偏转的角度(太复杂了)直接调用下方函数在分情况就好 
        ///                                                                       
        /// <summary>                                                             
        /// 返回根据需要偏转的角度                                                
        /// </summary>                                                            
        /// <param name="cur_x">当前点X坐标</param>                               
        /// <param name="cur_z">当前点Z坐标</param>                               
        /// <param name="dest_x">目标点X坐标</param>                              
        /// <param name="dest_z">目标点Y坐标</param>             //Z坐标          
        /// <param name="fish_rad"></param>                      //鱼的方向       
        /// <returns>所需偏转角度</returns>
        float Getxzdangle(float cur_x, float cur_z, float dest_x, float dest_z, float fish_rad)
                                                                                                
                                                                                                
        {                                                                                       
            float curangle;                                                                     
            curangle = (float)(Math.Abs(Math.Atan((cur_x - dest_x) / (cur_z - dest_z))));       
            if ((cur_x > dest_x) && (cur_z > dest_z))//以球为中心，当鱼在球右下角               
            {                                                                                   
                if (fish_rad > (-(Math.PI / 2 + curangle)) && fish_rad < (Math.PI / 2 - curangle))
                {                                                                                 
                    xzdangle = (float)(Math.PI / 2 + curangle + fish_rad);                        
                    xzdangle = -xzdangle;
                }
                else if (fish_rad > (Math.PI / 2 - curangle) && fish_rad < Math.PI)
                {
                    xzdangle = (float)(Math.PI * 1.5 - fish_rad - curangle);
                }
                else if (fish_rad < (-(Math.PI / 2 + curangle)) && fish_rad > -Math.PI)
                {
                    xzdangle = (float)(-Math.PI / 2 - curangle - fish_rad);
                }
            }
            else if ((cur_x > dest_x) && (cur_z < dest_z))//以球为中心，当鱼在球右上角
            {
                if (fish_rad < (Math.PI / 2 + curangle) && (-(Math.PI / 2 - curangle)) < fish_rad)
                {
                    xzdangle = (float)(Math.PI / 2 + curangle - fish_rad);
                }
                else if ((-(Math.PI / 2 - curangle)) > fish_rad && fish_rad > -Math.PI)
                {
                    xzdangle = (float)(Math.PI * 1.5 + fish_rad - curangle);
                    xzdangle = -xzdangle;
                }
                else if (fish_rad > (Math.PI / 2 + curangle) && fish_rad < Math.PI)
                {
                    xzdangle = (float)(fish_rad - curangle - Math.PI / 2);
                    xzdangle = -xzdangle;
                }
            }
            else if ((cur_x < dest_x) && (cur_z > dest_z))//以球为中心，当鱼在球左下角
            {
                if (fish_rad > -(Math.PI / 2 - curangle) && fish_rad < (Math.PI / 2 + curangle))
                {
                    xzdangle = (float)(Math.PI / 2 + fish_rad - curangle);
                    xzdangle = -xzdangle;
                }
                else if (fish_rad < -(Math.PI / 2 - curangle) && fish_rad > -Math.PI)
                {
                    xzdangle = (float)(curangle - fish_rad - Math.PI / 2);
                }
                else if (fish_rad > (Math.PI / 2 + curangle) && fish_rad < Math.PI)
                {
                    xzdangle = (float)(Math.PI * 1.5 + curangle - fish_rad);
                }
            }
            else if ((cur_x < dest_x) && (cur_z < dest_z))//以球为中心，当鱼在球左上角
            {
                if (fish_rad < (Math.PI / 2 - curangle) && fish_rad > -(Math.PI / 2 + curangle))
                {
                    xzdangle = (float)(Math.PI / 2 - curangle - fish_rad);

                }
                else if (fish_rad > (Math.PI / 2 - curangle) && fish_rad < Math.PI)
                {
                    xzdangle = (float)(fish_rad + curangle - Math.PI / 2);
                    xzdangle = -xzdangle;
                }
                else if (fish_rad < -(Math.PI / 2 + curangle) && fish_rad > -Math.PI)
                {
                    xzdangle = (float)(Math.PI * 1.5 + fish_rad + curangle);
                    xzdangle = -xzdangle;
                }
            }

            return xzdangle;
        }
        #endregion                                                                                          
        #region  根据需要返回一个偏转的角度,直接调用下方函数在分情况(自己的理解)
        /// <summary>                                                             
        /// 返回根据需要偏转的角度（分‘+’‘-’）                                                
        /// </summary>                                                            
        /// <param name="cur_x">当前点X坐标</param>                               
        /// <param name="cur_z">当前点Z坐标</param>                               
        /// <param name="dest_x">目标点X坐标</param>                              
        /// <param name="dest_z">目标点Y坐标</param>             //Z坐标          
        /// <param name="fish_rad"></param>                      //鱼的方向       
        /// <returns>所需偏转角度</returns>
         float MGetxzdangle(float cur_x, float cur_z, float dest_x, float dest_z, float fish_rad)                                                                                        
        {
            Angle=GetAnyangle(fcur_x,cur_z,dest_x,dest_z);
            Dangle=Angle-fishi_rad;
            if(Math.Abs(Angle-fishi_rad)<=Math.PI)
                  return Dangle;
            else
                {
                if(Dangle<0)
                    return 2*Math.PI+Dangle;
                else
                    return -2*Math.PI+Dangle;
                }
        }
        #endregion 


        #region  该函数返回任意两点间（当前点与目标点。方向为当前点指向目标点）连线与X轴正向夹角
        /// <summary>
        /// 该函数返回任意两点间（当前点与目标点。方向为当前点指向目标点）连线与X轴正向夹角
        /// </summary>add by 10级团队
        /// <param name="cur_x">当前点X坐标</param>
        /// <param name="cur_z">当前点Z坐标</param>
        /// <param name="dest_x">目标点X坐标</param>
        /// <param name="dest_z">目标点Z坐标</param>
        /// <param name="?"></param>
        /// <returns>与X轴正向夹角</returns>
        float GetAnyangle(float cur_x, float cur_z, float dest_x, float dest_z)
        {
            float curangle, anyangel;
            curangle = (float)(Math.Abs(Math.Atan((cur_x - dest_x) / (cur_z - dest_z))));
            if (cur_x > dest_x)
            {
                if (cur_z < dest_z)
                    anyangel = (float)(curangle + Math.PI / 2);                     //没错
                else
                    anyangel = (float)(-(curangle + Math.PI / 2));

            }
            else
            {

                if (cur_z < dest_z)
                    anyangel = (float)(Math.PI / 2 - curangle);
                else
                    anyangel = (float)(-(Math.PI / 2 - curangle));

            }
            return anyangel;


        }
        #endregion
        #region  该函数返回任意两点间（当前点与目标点。方向为当前点指向目标点）连线与X轴正向夹角(通过向量)
        /// <summary>
        /// 该函数返回任意两点间（当前点与目标点。方向为当前点指向目标点）连线与X轴正向夹角
        /// </summary>add by 10级团队
        /// <param name="cur_x">当前点X坐标</param>
        /// <param name="cur_z">当前点Z坐标</param>
        /// <param name="dest_x">目标点X坐标</param>
        /// <param name="dest_z">目标点Z坐标</param>
        /// <param name="?"></param>
        /// <returns>与X轴正向夹角</returns> 
        float GetAngel(float cur_x, float cur_z, float dest_x, float dest_z)
        {
 
        }
        
        #endregion

        #region 将弧度转换为角度    by 闯
        /// <summary>将弧度转换为角度
        /// 
        /// </summary>add by 兽之哀
        /// <param name="red"></param>弧度
        /// <returns></returns>角度
        float RedToAngle(float red)
        {
            return ((float)((red / Math.PI) * 180));
        }
        #endregion


        #region 将角度转化为弧度    by 闯
        /// <summary>将角度转化为弧度
        /// 
        /// </summary>add by 兽之哀
        /// <param name="angle"></param>角度
        /// <returns></returns>弧度
        float AngleToRed(float angle)
        {
            return ((float)((angle / 180) * Math.PI));
        }
        #endregion


        #region 此函数让第i号鱼的鱼体方向始终向着定点方向   by闯
        /// <summary>
        /// 此函数让第i号鱼的鱼体方向始终向着球
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="fish"></param>
        /// <param name="i"></param>
        /// <param name="teamId"></param>
        void Turn(Mission mission, ref Decision[] fish, float dest_x, float dest_z, int i, int teamId)
        {
            int angle = (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, dest_x, dest_z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
            if ((angle >= -1) && (angle <= 1))
            {
                decisions[i].TCode = 7;
                decisions[i].VCode = 0;
            }
            else if ((angle >= -8) && (angle < -1))
            {
                decisions[i].TCode = 7;
                decisions[i].VCode = 0;
            }
            else if ((angle >= -20) && (angle < -8))
            {
                decisions[i].TCode = 5;
                decisions[i].VCode = 1;
            }
            else if ((angle > -40) && (angle < -20))
            {
                decisions[i].TCode = 2;
                decisions[i].VCode = 1;
            }
            else if ((angle > -60) && (angle < -40))
            {
                decisions[i].TCode = 1;
                decisions[i].VCode = 1;
            }
            else if ((angle > -180) && (angle < -60))
            {
                decisions[i].TCode = 0;
                decisions[i].VCode = 1;
            }
            else if ((angle > 1) && (angle <= 8))
            {
                decisions[i].TCode = 7;
                decisions[i].VCode = 0;
            }
            else if ((angle > 8) && (angle <= 20))
            {
                decisions[i].TCode = 9;
                decisions[i].VCode = 1;
            }
            else if ((angle > 20) && (angle < 40))
            {
                decisions[i].TCode = 12;
                decisions[i].VCode = 1;
            }
            else if ((angle > 40) && (angle < 60))
            {
                decisions[i].TCode = 13;
                decisions[i].VCode = 1;
            }
            else if ((angle > 60) && (angle < 180))
            {
                decisions[i].TCode = 14;
                decisions[i].VCode = 1;
            }
            else
            {
                decisions[i].TCode = 7;
                decisions[i].VCode = 0;
            }
        }
        #endregion
        //有待理解

        #region 根据需要偏转的角度返回一个转弯档位

        /// <summary>
        /// 根据需要偏转的角度返回一个转弯档位
        /// </summary>
        /// <param name="angvel">所需偏的角度由求角度函数得到</param>
        /// <returns>T</returns>
        int AtoT(float angvel)
        {
            if (0 == angvel)
            {
                return 7;
            }
            else if (angvel < 0)
            {
                if (-0.005395 <= angvel && 0 > angvel)
                {
                    if ((0 - angvel) >= (angvel + 0.005395))
                        return 6;
                    else
                        return 7;

                }
                else if (-0.009016 <= angvel && -0.005395 > angvel)
                {
                    if ((-0.005395 - angvel) >= (angvel + 0.009016))
                        return 5;
                    else
                        return 6;

                }
                else if (-0.014203 <= angvel && -0.009016 > angvel)
                {
                    if ((-0.009016 - angvel) >= (angvel + 0.014203))
                        return 4;
                    else
                        return 5;
                }
                else if (-0.019907 <= angvel && -0.014203 > angvel)
                {
                    if ((-0.014203 - angvel) >= (angvel + 0.019907))
                        return 3;
                    else
                        return 4;
                }
                else if (-0.0253 <= angvel && -0.019907 > angvel)
                {
                    if ((-0.019907 - angvel) >= (angvel + 0.0253))
                        return 2;
                    else
                        return 3;
                }
                else if (-0.033592 <= angvel && -0.0253 > angvel)
                {
                    if ((-0.0253 - angvel) >= (angvel + 0.033592))
                        return 1;
                    else
                        return 2;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (0.005395 >= angvel && 0 < angvel)
                {
                    if (angvel - 0 > 0.005395 - angvel)
                        return 8;
                    else
                        return 7;

                }
                else if (0.009016 >= angvel && 0.005395 < angvel)
                {
                    if (angvel - 0.005395 > 0.009016 - angvel)
                        return 9;
                    else
                        return 8;
                }
                else if (0.014203 >= angvel && 0.009016 < angvel)
                {
                    if (angvel - 0.009016 > 0.014203 - angvel)
                        return 10;
                    else
                        return 9;
                }
                else if (0.019907 >= angvel && 0.014203 < angvel)
                {
                    if (angvel - 0.014203 > 0.019907 - angvel)
                        return 11;
                    else
                        return 10;
                }
                else if (0.0253 >= angvel && 0.019907 < angvel)
                {
                    if (angvel - 0.019907 > 0.0253 - angvel)
                        return 12;
                    else
                        return 11;
                }
                else if (0.033592 >= angvel && 0.0253 < angvel)
                {
                    if (angvel - 0.0253 > 0.033592 - angvel)
                        return 13;
                    else
                        return 12;
                }
                else if (0.040848 >= angvel && 0.033592 < angvel)
                {
                    if (angvel - 0.033592 > 0.040848 - angvel)
                        return 14;
                    else
                        return 13;
                }
                else
                {
                    return 14;
                }

            }
        }
        #endregion
        //不明白数据从哪里来的

        #region  返回Vector3类型的向量    LiYoubing 20110722
        /// <summary>
        /// 返回目标方向
        /// 场地坐标系定义为：X向右，Z向下，Y置0，负X轴顺时针转回负X轴角度范围为(-PI,PI)的坐标系
        /// 返回Vector3类型的向量（Y置0，只有X和Z有意义）在场地坐标系中方向的角度值
        /// </summary>
        /// <param name="v">待计算角度值的xna.Vector3类型向量</param>
        /// <returns>向量v在场地坐标系中方向的角度值</returns>
        public static float GetAngleDegree(xna.Vector3 v)
        {
            float x = v.X;
            float y = v.Z;
            float angle = 0;

            if (Math.Abs(x) < float.Epsilon)
            {// x = 0 直角反正切不存在
                if (Math.Abs(y) < float.Epsilon) { angle = 0.0f; }
                else if (y > 0) { angle = 90.0f; }
                else if (y < 0) { angle = -90.0f; }
            }
            else if (x < 0)                                                                             //个人感觉用y区分会更清楚
            {// x < 0 (90,180]或(-180,-90)
                if (y >= 0) { angle = (float)(180 * Math.Atan(y / x) / Math.PI) + 180.0f; }
                else { angle = (float)(180 * Math.Atan(y / x) / Math.PI) - 180.0f; }
            }
            else
            {// x > 0 (-90,90)
                angle = (float)(180 * Math.Atan(y / x) / Math.PI);
            }

            return angle;
        }

        #endregion


        #region 此函数让i鱼游向指定点    by 闯
        /// <summary>
        /// 此函数让i鱼游向指定点
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="fish"></param>
        /// <param name="i"></param>
        /// <param name="dest_x"></param>
        /// <param name="dest_z"></param>
        /// <param name="teamId"></param>
        void SwimToDest(Mission mission, ref Decision[] fish, int i, float dest_x, float dest_z, int n, int teamId)// 参数n是什么不太明白
        {
            decisions[i].TCode = AtoT(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, dest_x, dest_z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
            decisions[i].VCode = 5;
            if (n > 7) n = 0;
            int angle = (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, dest_x, dest_z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
            if ((angle > -1) && (angle < 1))
            {
                decisions[i].TCode = 7;
                decisions[i].VCode = 14 - n;
            }
            else if ((angle > -5) && (angle < -1))
            {
                decisions[i].TCode = 7;
                decisions[i].VCode = 14 - n;
            }
            else if ((angle > -20) && (angle < -5))
            {
                decisions[i].TCode = 4;
                decisions[i].VCode = 12 - n;
            }
            else if ((angle > -40) && (angle < -20))
            {
                decisions[i].TCode = 1;
                decisions[i].VCode = 10 - n;
            }
            else if ((angle > -60) && (angle < -40))
            {
                decisions[i].TCode = 0;
                decisions[i].VCode = 8 - n;
            }
            else if ((angle > -180) && (angle < -60))
            {
                decisions[i].TCode = 0;
                decisions[i].VCode = 4;
            }
            else if ((angle > 1) && (angle < 5))
            {
                decisions[i].TCode = 7;
                decisions[i].VCode = 14 - n;
            }
            else if ((angle > 5) && (angle < 20))
            {
                decisions[i].TCode = 10;
                decisions[i].VCode = 12 - n;
            }
            else if ((angle > 20) && (angle < 40))
            {
                decisions[i].TCode = 13;
                decisions[i].VCode = 10 - n;
            }
            else if ((angle > 40) && (angle < 60))
            {
                decisions[i].TCode = 14;
                decisions[i].VCode = 8 - n;
            }
            else if ((angle > 60) && (angle < 180))
            {
                decisions[i].TCode = 14;
                decisions[i].VCode = 4;
            }
        }
        #endregion
        //看不懂V得到的依据


        #region 此函数让i鱼游向指定点1    by 闯
        /// <summary>
        /// 此函数让i鱼游向指定点
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="fish"></param>
        /// <param name="i"></param>
        /// <param name="dest_x"></param>
        /// <param name="dest_z"></param>
        /// <param name="teamId"></param>
        void SSwimToDest(Mission mission, ref Decision[] fish, int i, float dest_x, float dest_z, int n, int teamId)
        {
            decisions[i].TCode = AtoT(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, dest_x, dest_z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
            decisions[i].VCode = 5;
            if (n > 7) n = 0;
            int angle = (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, dest_x, dest_z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
            if ((angle > -1) && (angle < 1))
            {
                decisions[i].TCode = 6;
                decisions[i].VCode = 14 - n;
            }
            else if ((angle > -5) && (angle < -1))
            {
                decisions[i].TCode = 5;
                decisions[i].VCode = 14 - n;
            }
            else if ((angle > -20) && (angle < -5))
            {
                decisions[i].TCode = 4;
                decisions[i].VCode = 13 - n;
            }
            else if ((angle > -40) && (angle < -20))
            {
                decisions[i].TCode = 1;
                decisions[i].VCode = 11 - n;
            }
            else if ((angle > -60) && (angle < -40))
            {
                decisions[i].TCode = 0;
                decisions[i].VCode = 9 - n;
            }
            else if ((angle > -180) && (angle < -60))
            {
                decisions[i].TCode = 0;
                decisions[i].VCode = 4;
            }
            else if ((angle > 1) && (angle < 5))
            {
                decisions[i].TCode = 8;
                decisions[i].VCode = 14 - n;
            }
            else if ((angle > 5) && (angle < 20))
            {
                decisions[i].TCode = 10;
                decisions[i].VCode = 13 - n;
            }
            else if ((angle > 20) && (angle < 40))
            {
                decisions[i].TCode = 13;
                decisions[i].VCode = 11 - n;
            }
            else if ((angle > 40) && (angle < 60))
            {
                decisions[i].TCode = 14;
                decisions[i].VCode = 9 - n;
            }
            else if ((angle > 60) && (angle < 180))
            {
                decisions[i].TCode = 14;
                decisions[i].VCode = 4;
            }
        }
        #endregion

        #region 让i号鱼静止
        /// <summary>
        /// 让i号鱼静止
        /// </summary>
        /// <param name="i"></param>
        void Stop(int i)
        {
            decisions[i].TCode = 7;
            decisions[i].VCode = 0;
        }
        #endregion


        #region 该函数命令己方的第i条鱼撞击对方的第j条鱼    by闯
        /// <summary>
        /// 该函数命令己方的第i条鱼去撞击对方的第j条鱼
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="fish"></param>
        /// <param name="i"></param>己方鱼标号
        /// <param name="j"></param>对方鱼标号
        /// <param name="where"></param>撞击部位（where值为“1”则撞击头部，否则撞击鱼体）
        /// <param name="teamId"></param>
        void Attack(Mission mission, ref Decision[] fish, int i, int j, int where, int teamId)
        {

            int angle = 0;

            if (where == 1)
            {
                decisions[i].TCode = AtoT(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PolygonVertices[0].X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PolygonVertices[0].Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
                decisions[i].VCode = 6;
                angle = (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PolygonVertices[0].X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PolygonVertices[0].Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
                if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) > 900)
                {
                    if ((angle > -5) && (angle < -3))
                    {
                        decisions[i].TCode = 4;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -20) && (angle < -5))
                    {
                        decisions[i].TCode = 1;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -40) && (angle < -20))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > -60) && (angle < -40))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 8;
                    }
                    else if ((angle > -180) && (angle < -60))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 5;
                    }
                    else if ((angle > 3) && (angle < 5))
                    {
                        decisions[i].TCode = 10;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 5) && (angle < 20))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > 20) && (angle < 40))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 11;
                    }
                    else if ((angle > 40) && (angle < 60))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 8;
                    }
                    else if ((angle > 60) && (angle < 180))
                    {
                        decisions[i].TCode = 13;
                        decisions[i].VCode = 5;
                    }
                }
                else if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) <= 900)
                {
                    if ((angle > -5) && (angle < -3))
                    {
                        decisions[i].TCode = 6;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -10) && (angle < -5))
                    {
                        decisions[i].TCode = 5;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -20) && (angle < -10))
                    {
                        decisions[i].TCode = 2;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > -40) && (angle < -20))
                    {
                        decisions[i].TCode = 1;
                        decisions[i].VCode = 11;
                    }
                    else if ((angle > -60) && (angle < -40))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 8;
                    }
                    else if ((angle > -180) && (angle < -60))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 7;
                    }
                    else if ((angle > 3) && (angle < 5))
                    {
                        decisions[i].TCode = 8;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 5) && (angle < 10))
                    {
                        decisions[i].TCode = 9;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 10) && (angle < 20))
                    {
                        decisions[i].TCode = 12;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > 20) && (angle < 40))
                    {
                        decisions[i].TCode = 13;
                        decisions[i].VCode = 11;
                    }
                    else if ((angle > 40) && (angle < 60))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 8;
                    }
                    else if ((angle > 60) && (angle < 180))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 7;
                    }
                }
                else if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) <= 700)
                {
                    if ((angle > -20) && (angle < -1))
                    {
                        decisions[i].TCode = 2;
                        decisions[i].VCode = 14;
                    }

                    else if ((angle > -40) && (angle < -20))
                    {
                        decisions[i].TCode = 1;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -60) && (angle < -40))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -180) && (angle < -60))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > 1) && (angle < 20))
                    {
                        decisions[i].TCode = 12;
                        decisions[i].VCode = 14;
                    }

                    else if ((angle > 20) && (angle < 40))
                    {
                        decisions[i].TCode = 13;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 40) && (angle < 60))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 60) && (angle < 180))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 13;
                    }
                }
                else if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) < 600)
                {
                    if ((angle > -40 && (angle < -1)))
                    {
                        decisions[i].TCode = 3;
                        decisions[i].VCode = 14;
                    }


                    else if ((angle > -60) && (angle < -40))
                    {
                        decisions[i].TCode = 1;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -180) && (angle < -60))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 12;
                    }
                    else if ((angle > 1) && (angle < 40))
                    {
                        decisions[i].TCode = 11;
                        decisions[i].VCode = 14;
                    }

                    else if ((angle > 40) && (angle < 60))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > 60) && (angle < 180))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 12;
                    }
                }
                else if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) < 380)
                {
                    if ((angle > -60 && (angle < -1)))
                    {
                        decisions[i].TCode = 1;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -180) && (angle < -60))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > 1) && (angle < 60))
                    {
                        decisions[i].TCode = 13;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 60) && (angle < 180))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 13;
                    }



                    else
                    {
                        decisions[i].TCode = AtoT(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
                        decisions[i].VCode = 6;
                        angle = (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
                        if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) > 800)
                        {
                            if ((angle > -5) && (angle < -3))
                            {
                                decisions[i].TCode = 7;
                                decisions[i].VCode = 14;
                            }
                            else if ((angle > -20) && (angle < -5))
                            {
                                decisions[i].TCode = 3;
                                decisions[i].VCode = 12;
                            }
                            else if ((angle > -40) && (angle < -20))
                            {
                                decisions[i].TCode = 2;
                                decisions[i].VCode = 2;
                            }
                            else if ((angle > -60) && (angle < -40))
                            {
                                decisions[i].TCode = 1;
                                decisions[i].VCode = 2;
                            }
                            else if ((angle > -180) && (angle < -60))
                            {
                                decisions[i].TCode = 0;
                                decisions[i].VCode = 2;
                            }
                            else if ((angle > 3) && (angle < 5))
                            {
                                decisions[i].TCode = 7;
                                decisions[i].VCode = 14;
                            }
                            else if ((angle > 5) && (angle < 20))
                            {
                                decisions[i].TCode = 11;
                                decisions[i].VCode = 12;
                            }
                            else if ((angle > 20) && (angle < 40))
                            {
                                decisions[i].TCode = 12;
                                decisions[i].VCode = 2;
                            }
                            else if ((angle > 40) && (angle < 60))
                            {
                                decisions[i].TCode = 13;
                                decisions[i].VCode = 2;
                            }
                            else if ((angle > 60) && (angle < 180))
                            {
                                decisions[i].TCode = 14;
                                decisions[i].VCode = 2;
                            }
                        }
                        else if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) <= 800)
                        {
                            if ((angle > -5) && (angle < -3))
                            {
                                decisions[i].TCode = 7;
                                decisions[i].VCode = 14;
                            }
                            else if ((angle > -20) && (angle < -5))
                            {
                                decisions[i].TCode = 5;
                                decisions[i].VCode = 12;
                            }
                            else if ((angle > -40) && (angle < -20))
                            {
                                decisions[i].TCode = 2;
                                decisions[i].VCode = 1;
                            }
                            else if ((angle > -60) && (angle < -40))
                            {
                                decisions[i].TCode = 1;
                                decisions[i].VCode = 1;
                            }
                            else if ((angle > -180) && (angle < -60))
                            {
                                decisions[i].TCode = 0;
                                decisions[i].VCode = 1;
                            }
                            else if ((angle > 3) && (angle < 5))
                            {
                                decisions[i].TCode = 7;
                                decisions[i].VCode = 14;
                            }
                            else if ((angle > 5) && (angle < 20))
                            {
                                decisions[i].TCode = 10;
                                decisions[i].VCode = 12;
                            }
                            else if ((angle > 20) && (angle < 40))
                            {
                                decisions[i].TCode = 12;
                                decisions[i].VCode = 1;
                            }
                            else if ((angle > 40) && (angle < 60))
                            {
                                decisions[i].TCode = 13;
                                decisions[i].VCode = 1;
                            }
                            else if ((angle > 60) && (angle < 180))
                            {
                                decisions[i].TCode = 14;
                                decisions[i].VCode = 1;
                            }


                        }

                    }
                }


            }
        }
        #endregion


        #region 该函数命令己方的第i条鱼撞击对方的第j条鱼的外围    by闯
        /// <summary>
        /// 该函数命令己方的第i条鱼去撞击对方的第j条鱼
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="fish"></param>
        /// <param name="i"></param>己方鱼标号
        /// <param name="j"></param>对方鱼标号
        /// <param name="where"></param>撞击部位（where值为“1”则撞击头部，否则撞击鱼体）
        /// <param name="teamId"></param>
        void Atouch(Mission mission, ref Decision[] fish, int i, int j, int where, int teamId)
        {

            int angle = 0;

            if (where == 1)
            {
                decisions[i].TCode = AtoT(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PolygonVertices[0].X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PolygonVertices[0].Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
                decisions[i].VCode = 6;
                angle = (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PolygonVertices[0].X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PolygonVertices[0].Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
                if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) > 800)
                {
                    if ((angle > -5) && (angle < -3))
                    {
                        decisions[i].TCode = 4;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -20) && (angle < -5))
                    {
                        decisions[i].TCode = 1;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -40) && (angle < -20))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > -60) && (angle < -40))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 8;
                    }
                    else if ((angle > -180) && (angle < -60))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 5;
                    }
                    else if ((angle > 3) && (angle < 5))
                    {
                        decisions[i].TCode = 10;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 5) && (angle < 20))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > 20) && (angle < 40))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 11;
                    }
                    else if ((angle > 40) && (angle < 60))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 8;
                    }
                    else if ((angle > 60) && (angle < 180))
                    {
                        decisions[i].TCode = 13;
                        decisions[i].VCode = 5;
                    }
                }
                else if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) <= 800)
                {
                    if ((angle > -5) && (angle < -3))
                    {
                        decisions[i].TCode = 5;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -10) && (angle < -5))
                    {
                        decisions[i].TCode = 5;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -20) && (angle < -10))
                    {
                        decisions[i].TCode = 2;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > -40) && (angle < -20))
                    {
                        decisions[i].TCode = 1;
                        decisions[i].VCode = 8;
                    }
                    else if ((angle > -60) && (angle < -40))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 6;
                    }
                    else if ((angle > -180) && (angle < -60))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 4;
                    }
                    else if ((angle > 3) && (angle < 5))
                    {
                        decisions[i].TCode = 7;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 5) && (angle < 10))
                    {
                        decisions[i].TCode = 12;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 10) && (angle < 20))
                    {
                        decisions[i].TCode = 12;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > 20) && (angle < 40))
                    {
                        decisions[i].TCode = 13;
                        decisions[i].VCode = 6;
                    }
                    else if ((angle > 40) && (angle < 60))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 5;
                    }
                    else if ((angle > 60) && (angle < 180))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 4;
                    }
                }
                else if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) < 550)
                {
                    if ((angle > -3) && (angle < -1))
                    {
                        decisions[i].TCode = 4;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -5) && (angle < -3))
                    {
                        decisions[i].TCode = 5;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -10) && (angle < -5))
                    {
                        decisions[i].TCode = 4;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -20) && (angle < -10))
                    {
                        decisions[i].TCode = 4;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > -40) && (angle < -20))
                    {
                        decisions[i].TCode = 3;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > -60) && (angle < -40))
                    {
                        decisions[i].TCode = 3;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > -180) && (angle < -60))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 9;
                    }
                    else if ((angle > 1) && (angle < 3))
                    {
                        decisions[i].TCode = 9;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 3) && (angle < 5))
                    {
                        decisions[i].TCode = 9;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 5) && (angle < 10))
                    {
                        decisions[i].TCode = 10;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 10) && (angle < 20))
                    {
                        decisions[i].TCode = 10;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > 20) && (angle < 40))
                    {
                        decisions[i].TCode = 11;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > 40) && (angle < 60))
                    {
                        decisions[i].TCode = 11;
                        decisions[i].VCode = 12;
                    }
                    else if ((angle > 60) && (angle < 180))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 9;
                    }

                    else if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) < 450)
                    {
                        if ((angle > -20) && (angle < -1))
                        {
                            decisions[i].TCode = 5;
                            decisions[i].VCode = 14;

                        }
                        else if ((angle > -40) && (angle < -20))
                        {
                            decisions[i].TCode = 4;
                            decisions[i].VCode = 13;

                        }
                        else if ((angle > -60) && (angle < -40))
                        {
                            decisions[i].TCode = 3;
                            decisions[i].VCode = 12;

                        }

                        else if ((angle > -180) && (angle < -60))
                        {
                            decisions[i].TCode = 0;
                            decisions[i].VCode = 11;

                        }
                        else if ((angle > 1) && (angle < 20))
                        {
                            decisions[i].TCode = 9;
                            decisions[i].VCode = 14;

                        }
                        else if ((angle > 20) && (angle < 40))
                        {
                            decisions[i].TCode = 10;
                            decisions[i].VCode = 13;

                        }
                        else if ((angle > 40) && (angle < 60))
                        {
                            decisions[i].TCode = 11;
                            decisions[i].VCode = 12;

                        }
                        else if ((angle > 60) && (angle < 180))
                        {
                            decisions[i].TCode = 14;
                            decisions[i].VCode = 11;

                        }
                        else if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) < 350)
                        {
                            if ((angle > -30) && (angle < -1))
                            {
                                decisions[i].TCode = 3;
                                decisions[i].VCode = 14;
                                decisions[i].VCode = 14;

                            }
                            else if ((angle > -60) && (angle < -30))
                            {
                                decisions[i].TCode = 2;
                                decisions[i].VCode = 13;
                                decisions[i].VCode = 13;
                            }

                            else if ((angle > -180) && (angle < -60))
                            {
                                decisions[i].TCode = 0;
                                decisions[i].VCode = 12;
                                decisions[i].VCode = 12;

                            }
                            else if ((angle > 1) && (angle < 30))
                            {
                                decisions[i].TCode = 11;
                                decisions[i].VCode = 14;
                                decisions[i].VCode = 14;

                            }
                            else if ((angle > 30) && (angle < 60))
                            {
                                decisions[i].TCode = 12;
                                decisions[i].VCode = 13;
                                decisions[i].VCode = 13;

                            }
                            else if ((angle > 60) && (angle < 180))
                            {
                                decisions[i].TCode = 14;
                                decisions[i].VCode = 12;
                                decisions[i].VCode = 12;

                            }
                            else if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) < 260)
                            {
                                if ((angle > -60) && (angle < -1))
                                {
                                    decisions[i].TCode = 2;
                                    decisions[i].VCode = 14;
                                    decisions[i].VCode = 14;

                                }


                                else if ((angle > -180) && (angle < -60))
                                {
                                    decisions[i].TCode = 0;
                                    decisions[i].VCode = 13;
                                    decisions[i].VCode = 13;

                                }
                                else if ((angle > 1) && (angle < 60))
                                {
                                    decisions[i].TCode = 12;
                                    decisions[i].VCode = 14;
                                    decisions[i].VCode = 14;

                                }

                                else if ((angle > 60) && (angle < 180))
                                {
                                    decisions[i].TCode = 14;
                                    decisions[i].VCode = 13;
                                    decisions[i].VCode = 13;

                                }
                            }
                        }
                    }
                }
            }
            else
            {
                decisions[i].TCode = AtoT(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
                decisions[i].VCode = 6;
                angle = (int)RedToAngle(Getxzdangle(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z, mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad));
                if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) > 800)
                {
                    if ((angle > -5) && (angle < -3))
                    {
                        decisions[i].TCode = 7;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -20) && (angle < -5))
                    {
                        decisions[i].TCode = 3;
                        decisions[i].VCode = 12;
                    }
                    else if ((angle > -40) && (angle < -20))
                    {
                        decisions[i].TCode = 2;
                        decisions[i].VCode = 2;
                    }
                    else if ((angle > -60) && (angle < -40))
                    {
                        decisions[i].TCode = 1;
                        decisions[i].VCode = 2;
                    }
                    else if ((angle > -180) && (angle < -60))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 2;
                    }
                    else if ((angle > 3) && (angle < 5))
                    {
                        decisions[i].TCode = 7;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 5) && (angle < 20))
                    {
                        decisions[i].TCode = 11;
                        decisions[i].VCode = 12;
                    }
                    else if ((angle > 20) && (angle < 40))
                    {
                        decisions[i].TCode = 12;
                        decisions[i].VCode = 2;
                    }
                    else if ((angle > 40) && (angle < 60))
                    {
                        decisions[i].TCode = 13;
                        decisions[i].VCode = 2;
                    }
                    else if ((angle > 60) && (angle < 180))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 2;
                    }
                }
                else if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) <= 800)
                {
                    if ((angle > -5) && (angle < -3))
                    {
                        decisions[i].TCode = 7;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -20) && (angle < -5))
                    {
                        decisions[i].TCode = 5;
                        decisions[i].VCode = 12;
                    }
                    else if ((angle > -40) && (angle < -20))
                    {
                        decisions[i].TCode = 2;
                        decisions[i].VCode = 1;
                    }
                    else if ((angle > -60) && (angle < -40))
                    {
                        decisions[i].TCode = 1;
                        decisions[i].VCode = 1;
                    }
                    else if ((angle > -180) && (angle < -60))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 1;
                    }
                    else if ((angle > 3) && (angle < 5))
                    {
                        decisions[i].TCode = 7;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 5) && (angle < 20))
                    {
                        decisions[i].TCode = 10;
                        decisions[i].VCode = 12;
                    }
                    else if ((angle > 20) && (angle < 40))
                    {
                        decisions[i].TCode = 12;
                        decisions[i].VCode = 1;
                    }
                    else if ((angle > 40) && (angle < 60))
                    {
                        decisions[i].TCode = 13;
                        decisions[i].VCode = 1;
                    }
                    else if ((angle > 60) && (angle < 180))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 1;
                    }
                }
                else if (GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[i].PositionMm.X, mission.TeamsRef[teamId].Fishes[i].PositionMm.Z, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.X, mission.TeamsRef[(1 + teamId) % 2].Fishes[j].PositionMm.Z) < 300)
                {
                    if ((angle > -1) && (angle < 1))
                    {
                        decisions[i].TCode = 7;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -3) && (angle < -1))
                    {
                        decisions[i].TCode = 7;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -5) && (angle < -3))
                    {
                        decisions[i].TCode = 7;
                        decisions[i].VCode = 13;
                    }
                    else if ((angle > -10) && (angle < -5))
                    {
                        decisions[i].TCode = 6;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > -20) && (angle < -10))
                    {
                        decisions[i].TCode = 5;
                        decisions[i].VCode = 12;
                    }
                    else if ((angle > -40) && (angle < -20))
                    {
                        decisions[i].TCode = 2;
                        decisions[i].VCode = 12;
                    }
                    else if ((angle > -60) && (angle < -40))
                    {
                        decisions[i].TCode = 1;
                        decisions[i].VCode = 1;
                    }
                    else if ((angle > -180) && (angle < -60))
                    {
                        decisions[i].TCode = 0;
                        decisions[i].VCode = 1;
                    }
                    else if ((angle > 1) && (angle < 3))
                    {
                        decisions[i].TCode = 7;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 3) && (angle < 5))
                    {
                        decisions[i].TCode = 7;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 5) && (angle < 10))
                    {
                        decisions[i].TCode = 8;
                        decisions[i].VCode = 14;
                    }
                    else if ((angle > 10) && (angle < 20))
                    {
                        decisions[i].TCode = 9;
                        decisions[i].VCode = 12;
                    }
                    else if ((angle > 20) && (angle < 40))
                    {
                        decisions[i].TCode = 12;
                        decisions[i].VCode = 12;
                    }
                    else if ((angle > 40) && (angle < 60))
                    {
                        decisions[i].TCode = 13;
                        decisions[i].VCode = 1;
                    }
                    else if ((angle > 60) && (angle < 180))
                    {
                        decisions[i].TCode = 14;
                        decisions[i].VCode = 1;

                    }
                }
            }
        }
     
        #endregion 


        #region 返回距离我方进攻鱼最近的躲避鱼
        int minlength(Mission mission, ref Decision[] fish, int b1, int b2, int b3, int teamId)
        {
            int i=1;
            float l1 = GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[0].PositionMm.X, mission.TeamsRef[teamId].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].PositionMm.Z);
            float l2 = GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[0].PositionMm.X, mission.TeamsRef[teamId].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].PositionMm.Z);
            float l3 = GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[0].PositionMm.X, mission.TeamsRef[teamId].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].PositionMm.Z);
            if(TtoT(mission, ref decisions, 0, 20) == true)
            {
                i = 1;
            }
            else if(TtoT(mission, ref decisions, 20, 300) == true)
            {
                if (b1 != 1 && b2 != 1 && b3 != 1)
                {
                    if (l1 > l2)
                        if (l2 > l3)
                            i = 3;
                    if (l1 > l2)
                        if (l2 < l3)
                            i = 2;
                    if (l1 < l2)
                        if (l1 < l3)
                            i = 1;
                    if (l1 < l2)
                        if (l1 > l3)
                            i = 3;
                }
                else if (b1 == 1 && b2 != 1 && b3 != 1)
                {
                    if (l2 >= l3)
                        i = 3;
                    else
                        i = 2;
                }
                else if (b1 != 1 && b2 == 1 && b3 != 1)
                {
                    if (l1 >= l3)
                        i = 3;
                    else
                        i = 1;
                }
                else if (b1 != 1 && b2 != 1 && b3 == 1)
                {
                    if (l1 >= l2)
                        i = 2;
                    else
                        i = 1;
                }
                else if (b1 == 1 && b2 == 1 && b3 != 1)
                    i = 3;
                else if (b1 == 1 && b2 != 1 && b3 == 1)
                    i = 2;
                else if (b1 != 1 && b2 == 1 && b3 == 1)
                    i = 1;
            }
           
            return i;
        }
        #endregion


        #region 返回距离对方进攻鱼最近的躲避鱼
        int eminlength(Mission mission, ref Decision[] fish, float b1, float b2, float b3, int teamId)
        {
            int i = 1;
            float l1 = GetLengthToDestpoint(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z, mission.TeamsRef[teamId].Fishes[1].PositionMm.X, mission.TeamsRef[teamId].Fishes[1].PositionMm.Z);
            float l2 = GetLengthToDestpoint(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z, mission.TeamsRef[teamId].Fishes[2].PositionMm.X, mission.TeamsRef[teamId].Fishes[2].PositionMm.Z);
            float l3 = GetLengthToDestpoint(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z, mission.TeamsRef[teamId].Fishes[3].PositionMm.X, mission.TeamsRef[teamId].Fishes[3].PositionMm.Z);
            if (b1 == 1)
                l1 = l2 + l3;
            if (b2 == 1)
                l2 = l1 + l2;
            if (b3 == 1)
                l3 = l1 + l2;
            if (b1 != 1 && b2 != 1 && b3 != 1)
            {
                if (l1 > l2)
                    if (l3 > l2)
                        i = 2;
                if (l1 > l2)
                    if (l3 < l2)
                        i = 3;
                if (l1 < l2)
                    if (l3 > l1)
                        i = 1;
                if (l1 < l2)
                    if (l3 < l1)
                        i = 3;
            }
            else
            {
                if (b1 == 1 && b2 == 1 && b3 != 1)
                    i = 3;
                if (b1 == 1 && b3 == 1 && b2 != 1)
                    i = 2;
                if (b2 == 1 && b3 == 1 && b1 != 1)
                    i = 1;
                if (b1 == 1 && b2 != 1 && b3 != 1)
                {
                    if (l2 > l3)
                        i = 3;
                    else
                        i = 2;
                }
                if (b2 == 1 && b1 != 1 && b3 != 1)
                {
                    if (l1 > l3)
                        i = 3;
                    else
                        i = 1;
                }
                if (b3 == 1 && b1 != 1 && b2 != 1)
                {
                    if (l1 > l2)
                        i = 2;
                    else
                        i = 1;
                }
            }
            return i;
        }
        #endregion
        //b1,b2,b3  是什么参数不明白



        #region 返回三个坐标点之间两两连线的夹角，以第一个坐标为参考
        float returndir(float x1, float z1, float x2, float z2, float x3, float z3)
        {
            float l1 = GetLengthToDestpoint(x1, z1, x2, z2);
            float l2 = GetLengthToDestpoint(x1, z1, x3, z3);
            float l3 = GetLengthToDestpoint(x3, z3, x2, z2);
            float l4 = (float)Math.Abs(Math.Acos((l1 * l1 + l2 * l2 - l3 * l3) / (2 * l1 * l2)));
            return l4;
        }
        #endregion


        #region 冲去碰鱼
        void touch(Mission mission, ref Decision[] fish, int n, int teamId)
        {
            
            float fx = mission.TeamsRef[teamId].Fishes[0].PositionMm.X,
                  fz = mission.TeamsRef[teamId].Fishes[0].PositionMm.Z,
                  efx1 = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X,
                  efz1 = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z;
            float l1 = GetLengthToDestpoint(fx, fz, efx1, efz1);
            float l = GetLengthToDestpoint(fx, fz, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z);
            float dir = returndir(fx, fz, efx1, efz1, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z);
            float l3 = l1 * (float)Math.Cos(dir);
            float l4 = l1 * (float)Math.Sin(dir);
            //进攻鱼所需偏转的角度
            float dir1 = Math.Abs(Math.Abs(GetAnyangle(fx, fz, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z)) - Math.Abs(mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad));         
            float dir2 = Math.Abs(Math.Abs(GetAnyangle(efx1, efz1, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z)) - Math.Abs(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].BodyDirectionRad));           
            if (((l4 > l3)) || (dir > ((Math.PI) / 2)))
            {
                //大于350 判断是否需要穿孔
                if (l > 350)
                {
                    //判断是否需要躲避障碍物，鱼在障碍物对面：穿过障碍物
                    if ((fx > -400 && fx < -200 && mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X > 200/*&&mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X<350*/) || (fx > 200 && fx < 400/*&&mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X>-350*/&& mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X < -200))
                        go(mission, ref decisions, 0, n, teamId);
                    //鱼在同侧：游向躲避鱼
                    else
                    {

                        //if (TtoT(mission, ref decisions, mission.CommonPara.RemainingCycles, mission.CommonPara.RemainingCycles + 50))
                        //{
                            SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
                        //}
                       /* else
                        {
                            if ((TtoT(mission, ref decisions, mission.CommonPara.RemainingCycles, mission.CommonPara.RemainingCycles + 3))&&(mission.TeamsRef[teamId].Fishes[0].PositionMm.X > 0 && mission.TeamsRef[teamId].Fishes[0].PositionMm.X < 500 && mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X < 0 && mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X > -500)|| (mission.TeamsRef[teamId].Fishes[0].PositionMm.X < 0 && mission.TeamsRef[teamId].Fishes[0].PositionMm.X >- 500 && mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X > 0 && mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X < 500))
                           {
                               decisions[0].TCode = 0;
                               decisions[0].VCode = 1;
                            }
                            else
                            {
                                SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
                            }
                               
                        }*/
                    }
                }
                //小于350直接干
                else
                    Attack(mission, ref decisions, 0, n, 1, teamId);
            }
            else
            {
                if ((mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X > 0 && fx < 0) || (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X < 0 && fx > 0))
                    go(mission, ref decisions, 0, max(mission, ref decisions, teamId), teamId);
                    //穿过障碍物
                
                else

                    SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
            }       //直接上
        }
        #endregion


        private int t = 0;

        #region 冲去碰鱼2
        void touch2(Mission mission, ref Decision[] fish, int n, int teamId)
        {
            float fx = mission.TeamsRef[teamId].Fishes[0].PositionMm.X,
                 fz = mission.TeamsRef[teamId].Fishes[0].PositionMm.Z;
             //OutFile(@"C:\Users\Cheryl\Desktop\shake_flag.txt", " " + "t: " + t +  "\n");
            if (t < 20)
            {
                if (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].BodyDirectionRad > (Math.PI) / 9 && mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].BodyDirectionRad < 8 * (Math.PI) / 9 && (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X < -200 || mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X > 200))
                {
                    if (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X < -200)
                    {
                        SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X+100, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
                        t++;
                    }
                    else if (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X > 200)
                    {
                        SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X-100, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z , 0, teamId);
                        t++;
                    }

                }
                else if (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].BodyDirectionRad < -(Math.PI) / 9 && mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].BodyDirectionRad > -8 * (Math.PI) / 9 && (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X < -200 || mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X > 200))
                {
                    if (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X < -200)
                    {
                        SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X + 100, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z , 0, teamId);
                        t++;
                    }
                    else if (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X > 200)
                    {
                        SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X - 100, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z , 0, teamId);
                        t++;
                    }

                }
                //对方躲避鱼的位置
                else if (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X > -200 && mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X < 200)
                {
                    if (mission.TeamsRef[teamId].Fishes[0].PositionMm.X > -200 && mission.TeamsRef[teamId].Fishes[0].PositionMm.X < 200 && ((mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad > (Math.PI) / 4 && mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad < 3 * (Math.PI) / 4) || (mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad < -(Math.PI) / 4 && mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad > -3 * (Math.PI) / 4)))
                    {
                        decisions[0].TCode = 6;
                        decisions[0].VCode = 7;
                        if (mission.TeamsRef[teamId].Fishes[0].PositionMm.X > 200 || mission.TeamsRef[teamId].Fishes[0].PositionMm.X < -200)
                        {
                            if(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X>200)
                            {
                                SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X-100, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
                                t++;
                            }
                            else if (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X <- 200)
                            {
                                SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X + 100, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
                                t++;
                            }
                            else
                            {
                                SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X , mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
                                t++;
                            }
                               
                        }

                    }
                    else
                    {
                        if(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].BodyDirectionRad>0&& mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].BodyDirectionRad< (Math.PI))
                        {
                            SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z+100, 0, teamId);
                            t++;
                        }
                        else
                        {
                            SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z-100, 0, teamId);
                            t++;
                        }
                       
                    }

                }
                else
                {
                    decisions[0].TCode = 10;
                    decisions[0].VCode = 3;
                    if (mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad == (Math.PI) / 2 || mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad == -(Math.PI) / 2)
                    {
                        SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
                        t++;
                    }
                }
            }
            
            else if (t == 20)
            {
                
                if(mission.TeamsRef[teamId].Fishes[0].PositionMm.X< mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X)
                {
                    if ((mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad >= (Math.PI) / 2 || mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad <= -(Math.PI) / 2) && (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].BodyDirectionRad >= (Math.PI) / 2 || mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].BodyDirectionRad <= -(Math.PI) / 2))
                    {
                        decisions[0].TCode = 4;
                        decisions[0].VCode = 3;
                        if (mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad <= (Math.PI) / 2 || mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad >= -(Math.PI) / 2)
                        {
                            SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
                            t = 0;
                        }
                    }
                    else if ((mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad <= (Math.PI) / 2 || mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad >= -(Math.PI) / 2) && (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].BodyDirectionRad >= (Math.PI) / 2 || mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].BodyDirectionRad >= -(Math.PI) / 2))
                    {
                        decisions[0].TCode = 4;
                        decisions[0].VCode = 3;
                        if (mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad >= (Math.PI) / 2 || mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad <= -(Math.PI) / 2)
                        {
                            SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
                            t = 0;
                        }

                    }
                    else
                    {
                        SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
                        t = 0;
                    }


                }
                else 
                {
                    if ((mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad >= (Math.PI) / 2 || mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad <= -(Math.PI) / 2) && (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].BodyDirectionRad >= (Math.PI) / 2 || mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].BodyDirectionRad <= -(Math.PI) / 2))
                    {
                        decisions[0].TCode = 10;
                        decisions[0].VCode = 3;
                        if (mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad <= (Math.PI) / 2 || mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad >= -(Math.PI) / 2)
                        {
                            SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
                            t = 0;
                        }
                    }
                    else if ((mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad <= (Math.PI) / 2 || mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad >= -(Math.PI) / 2) && (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].BodyDirectionRad >= (Math.PI) / 2 || mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].BodyDirectionRad >= -(Math.PI) / 2))
                    {
                        decisions[0].TCode = 10;
                        decisions[0].VCode = 3;
                        if (mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad >= (Math.PI) / 2 || mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad <= -(Math.PI) / 2)
                        {
                            SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
                            t = 0;
                        }

                    }
                    else
                    {
                        SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[n].PositionMm.Z, 0, teamId);
                        t = 0;
                    }


                }
             

            }
          
        }

        #endregion


        #region 返回三个数里最大的一个
        int max(Mission mission, ref Decision[] fish, int teamId)
        {
            int d=2;
            float a = Math.Abs(GetLengthToDestpoint(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].PositionMm.Z));// - GetLengthToDestpoint(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].PositionMm.Z));
            float b = Math.Abs(GetLengthToDestpoint(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].PositionMm.Z));// - GetLengthToDestpoint(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].PositionMm.Z));
            float c = Math.Abs(GetLengthToDestpoint(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].PositionMm.Z));// - GetLengthToDestpoint(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].PositionMm.Z));
            if (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].PositionMm.Z > 1550)
                a = 0;
            if (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].PositionMm.Z > 1550)
                b = 0;
            if (mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].PositionMm.Z > 1550)
                c = 0;
            if (a > b)
                if (a > c)
                    d = 1;
            if (a > b)
                if (a < c)
                    d = 3;
            if (a < b)
                if (b > c)
                    d = 2;
            if (a < b)
                if (b < c)
                    d = 3;
            if (a < c)
                if (c > b)
                    d = 3;
            if (a < c)
                if (c < b)
                    d = 2;
            if (a > c)
                if (a > b)
                    d = 1;
            if (a > c)
                if (a < b)
                    d = 2;
            return d;
        }
        #endregion


        #region 避免障碍物的阻挡
        void go(Mission mission, ref Decision[] fish, int i, int j, int teamId)
        {
            float x = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[j].PositionMm.X,
                  z = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[j].PositionMm.Z,
                  //我方进攻鱼的距离到对方躲避鱼的距离减去防守鱼到躲避鱼的距离
                  a = Math.Abs(GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[0].PositionMm.X, mission.TeamsRef[teamId].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].PositionMm.Z) - GetLengthToDestpoint(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].PositionMm.Z)),
                  b = Math.Abs(GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[0].PositionMm.X, mission.TeamsRef[teamId].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].PositionMm.Z) - GetLengthToDestpoint(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].PositionMm.Z)),
                  c = Math.Abs(GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[0].PositionMm.X, mission.TeamsRef[teamId].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].PositionMm.Z) - GetLengthToDestpoint(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].PositionMm.Z)),
                  fish0X = mission.TeamsRef[teamId].Fishes[i].PositionMm.X,
                  fish0Z = mission.TeamsRef[teamId].Fishes[i].PositionMm.Z,
                  dir=mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad,
                  l1 = GetLengthToDestpoint(fish0X, fish0Z, 0, returns(mission, ref decisions, i, teamId)),
                  l = 600,
                  dir1,
                  dir2;
            if (x >= 0)
            {
                dir1 = GetAnyangle(0, returns(mission, ref decisions, i, teamId), 200, returns(mission, ref decisions, i, teamId) - 200);
                dir2 = GetAnyangle(0, returns(mission, ref decisions, i, teamId), 200, returns(mission, ref decisions, i, teamId) + 200);
            }
            else
            {
                dir1 = GetAnyangle(0, returns(mission, ref decisions, i, teamId), -200, returns(mission, ref decisions, i, teamId) - 200);
                dir2 = GetAnyangle(0, returns(mission, ref decisions, i, teamId), -200, returns(mission, ref decisions, i, teamId) + 200);
            }
            if (x >= 0)
            {
                if ((l1 <= l) /*&& (Math.Abs(dir) < Math.PI / 4)*/)
                {
                    if ((dir1 <= GetAnyangle(0, returns(mission, ref decisions, i, teamId), x, z)) && ((GetAnyangle(0, returns(mission, ref decisions, i, teamId), x, z) < 0) || mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[j].BodyDirectionRad==Math.PI))
                    {
                        decisions[i].VCode = 15;
                        SwimToDest(mission, ref decisions, i, 0, returns(mission, ref decisions, i, teamId) - 400, 0, teamId);
                    }
                    if ((dir2 >= GetAnyangle(0, returns(mission, ref decisions, i, teamId), x, z)) && (GetAnyangle(0, returns(mission, ref decisions, i, teamId), x, z) > 0))
                    {
                        decisions[i].VCode = 15;
                        SwimToDest(mission, ref decisions, i, 0, returns(mission, ref decisions, i, teamId) + 400, 0, teamId);
                    }
                }
                /*if(GetLengthToDestpoint(x, z, 0, returns(mission, ref decisions, i, teamId))<l)
                    SwimToDest(mission, ref decisions, i, 0, returns(mission, ref decisions, i, teamId) + 200, 0, teamId);*/
            }
            else
                if ((l1 <= l) /*&& (Math.Abs(dir) > Math.PI*3 / 4)*/)
                {
                    if ((dir1 > GetAnyangle(fish0X, fish0Z, x, z)) && (dir1 <= 0))
                    {
                        decisions[i].VCode = 15;
                        SwimToDest(mission, ref decisions, i, 150, returns(mission, ref decisions, i, teamId) - 400, 0, teamId);
                    }
                    if ((dir2 < GetAnyangle(fish0X, fish0Z, x, z)) && (dir2 > 0))
                    {
                        decisions[i].VCode = 15;
                        SwimToDest(mission, ref decisions, i, 150, returns(mission, ref decisions, i, teamId) + 400, 0, teamId);
                    }
                }
            if(l1>l)
                SwimToDest(mission, ref decisions, i, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[max(mission, ref decisions, teamId)].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[max(mission, ref decisions, teamId)].PositionMm.Z, 0, teamId);
        }
        #endregion


        #region 返回离鱼最近的障碍//的z坐标
        float returns(Mission mission, ref Decision[] fish,int i,int teamId)
        {
            float fish0X = mission.TeamsRef[teamId].Fishes[i].PositionMm.X;
            float fish0Z = mission.TeamsRef[teamId].Fishes[i].PositionMm.Z;
            float l1 = GetLengthToDestpoint(fish0X, fish0Z, 0, -700);
            float l2 = GetLengthToDestpoint(fish0X, fish0Z, 0, 0);
            float l3 = GetLengthToDestpoint(fish0X, fish0Z, 0, 700);
            float o=2;
            if(l1>l2)
                if(l2>l3)
                    o=700;
            else if(l1>l2)
                if(l2<l3)
                    o=0;
            else if(l1<l2)
                if(l1>l3)
                    o=700;
            else if(l1<l2)
                if(l1<l3)
                    o=-700;
            return o;
        }
        #endregion


        #region 返回以障碍物为中心确定躲避鱼的X坐标
        float returnX(float efx0, float efz0, int i,float R1)
        {
            float X,d=-700,q,s,x1,x2,a,b,c,y1,y2;
            if (i == 1)
                d = -700;
            if (i == 2)
                d = 0;
            if (i == 3)
                d = 700;
            a = 1 + ((float)Math.Pow(((efz0 - d) / efx0), 2));
            b = 0;
            c =  - R1 * R1;
            q=b*b-4*a*c;
	        s=(float)Math.Sqrt(q);
		    x1=(-b+s)/2/a;
		    x2=(-b-s)/2/a;
            y1 = (efz0 - d) * x1 / efx0 + d;
            y2 = (efz0 - d) * x2 / efx0 + d;
            if (GetLengthToDestpoint(efx0, efz0, x1, y1) > GetLengthToDestpoint(efx0, efz0, x2, y2))
                X = x1;
            else
                X = x2;
            return X;
        }
        #endregion


        #region 返回以障碍物为中心确定躲避鱼的Y坐标
        float returnY(float efx0, float efz0, int i,float R1)
        {
            float d=-700;
            if (i == 1)
                d = -700;
            if (i == 2)
                d = 0;
            if (i == 3)
                d = 700;
            return (efz0 - d) * returnX(efx0, efz0, i,R1) / efx0 + d;
        }
        #endregion


        #region 返回以给定点为圆心以及给定半径躲避鱼的X坐标
        float freturnX(float x0, float z0, float x1, float z1, float r)
        {
            float s = (z1 - z0) / (x1 - x0),
                  a = s * s + 1,
                  b = -(2 * s * s * x0 + 2 * x0),
                  c = (s * x0) * (s * x0) + x0 * x0 - r * r,
                  X1,X2,Y1,Y2;
            X1=(-b+(float)Math.Sqrt(b*b-4*a*c))/2/a;
            X2 = (-b - (float)Math.Sqrt(b * b - 4 * a * c)) / 2 / a;
            Y1=s*X1-s*x0+z0;
            Y2=s*X2-s*x0+z0;
            if (GetLengthToDestpoint(x1, z1, X1, Y1) > GetLengthToDestpoint(x1, z1, X2, Y2))
                return X1;
            else
                return X2;
        }
        #endregion


        #region 返回以给定点为圆心以及给定半径躲避鱼的Y坐标
        float freturnY(float x0, float z0, float x1, float z1, float r)
        {
            float X = freturnX(x0, z0, x1, z1, r),
                  s = (z1 - z0) / (x1 - x0);
            return (s * X - s * x0 + z0);
        }
        #endregion


        #region 返回三个数最小的一个
        float min(float a, float b, float c)
        {
            float d;
            if (a > b)
            {
                if (b > c)
                    d = c;
                else
                    d = b;
            }
            else
            {
                if (a > c)
                    d = c;
                else
                    d = a;
            }
            return d;
        }
        #endregion


        #region 返回我方躲避鱼与对方攻击鱼之间的夹角（取小于180度的那个）
        float returndir1(Mission mission, ref Decision[] fish, int i, int teamId)
        {
            float dir = mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad,
                  edir = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].BodyDirectionRad,
                  d;
            if ((dir > 0 && edir > 0)||(dir < 0 && edir < 0))
                d = Math.Abs(dir - edir);
            else
            {
                if ((Math.Abs(dir) + Math.Abs(edir)) < Math.PI)
                    d = Math.Abs(dir) + Math.Abs(edir);
                else
                    d = (float)Math.PI * 2 - Math.Abs(dir) - Math.Abs(edir);
            }
            return d;
        }
        #endregion


        #region 返回我方攻击鱼与对方躲避鱼之间的夹角（取小于180度的那个）
        float ereturndir1(Mission mission, ref Decision[] fish, int i, int teamId)
        {
            float edir = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[i].BodyDirectionRad,
                  dir = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad,
                  d;
            if ((edir > 0 && dir > 0) || (edir < 0 && dir < 0))
                d = Math.Abs(edir - dir);
            else
            {
                if ((Math.Abs(edir) + Math.Abs(dir)) < Math.PI)
                    d = Math.Abs(edir) + Math.Abs(dir);
                else
                    d = (float)Math.PI * 2 - Math.Abs(edir) - Math.Abs(dir);
            }
            return d;
        }
        #endregion


        #region 判断鱼尾是否在对方攻击鱼的鱼体方向延长线上
        int judge(Mission mission, ref Decision[] fish, int i, int teamId)
        {
            float efish0Z = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z,
                  efish0X = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X,
                  edir = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].BodyDirectionRad,
                  dir = mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad,
                  fishX = mission.TeamsRef[teamId].Fishes[i].PositionMm.X,
                  fishZ = mission.TeamsRef[teamId].Fishes[i].PositionMm.Z;
            float x = ((float)Math.Tan(edir) * efish0X - (float)Math.Tan(dir) * fishX + fishZ - efish0Z) / ((float)Math.Tan(edir) - (float)Math.Tan(dir)),
                  y = (float)Math.Tan(edir) * x - (float)Math.Tan(edir) * efish0X + efish0Z,
                  l = GetLengthToDestpoint(x, y, fishX, fishZ);
            if (l < 900)
                return 1;
            else
                return 0;
        }
        #endregion


        #region 判断在一定距离内对方躲避鱼鱼尾是否在我方攻击鱼的延长线上
        int ejudge(Mission mission, ref Decision[] fish, int i, int teamId)
        {
            float fishZ = mission.TeamsRef[teamId].Fishes[0].PositionMm.Z,
                  fishX = mission.TeamsRef[teamId].Fishes[0].PositionMm.X,
                  dir = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad,
                  edir = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[i].BodyDirectionRad,
                  efishX = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[i].PositionMm.X,
                  efishZ = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[i].PositionMm.Z;
            float x = ((float)Math.Tan(dir) * fishX - (float)Math.Tan(edir) * efishX + efishZ - fishZ) / ((float)Math.Tan(dir) - (float)Math.Tan(edir)),
                  y = (float)Math.Tan(dir) * x - (float)Math.Tan(dir) * fishX + fishZ,
                  l = GetLengthToDestpoint(x, y, efishX, efishZ);
            //这个算法完全看不懂= =
            if (l < 100)
            {
                if (GetLengthToDestpoint(fishX, fishZ, efishX, efishZ) < 450)
                    return 1;
                else
                    return 0;
            }
            else if (l > 100 && l < 150)
            {
                if (GetLengthToDestpoint(fishX, fishZ, efishX, efishZ) < 400)
                    return 1;
                else
                    return 0;
            }
            else if (l > 150 && l < 200)
            {
                if (GetLengthToDestpoint(fishX, fishZ, efishX, efishZ) < 350)
                    return 1;
                else
                    return 0;
            }
            else if (l > 200 && l < 250)
            {
                if (GetLengthToDestpoint(fishX, fishZ, efishX, efishZ) < 300)
                    return 1;
                else
                    return 0;
            }
            else if (l > 250 && l < 300)
            {
                if (GetLengthToDestpoint(fishX, fishZ, efishX, efishZ) < 250)
                    return 1;
                else
                    return 0;
            }
            else
                return 0;
        }
        #endregion


        #region 判断目标对方躲避鱼在我方进攻鱼的左方还是右方
        int judgeLorR(Mission mission, ref Decision[] fish, int i, int teamId)
        {
            float fishX = mission.TeamsRef[teamId].Fishes[0].PositionMm.X,
                  fishZ = mission.TeamsRef[teamId].Fishes[0].PositionMm.Z,
                  efishX = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[i].PositionMm.X,
                  efishZ = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[i].PositionMm.Z,
                  dir1 = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad,
                  y;
            y = (float)Math.Tan(dir1) * efishX - (float)Math.Tan(dir1) * fishX + fishZ;
            if (dir1 > -Math.PI / 2 && dir1 < Math.PI / 2)
            {
                if (y > efishZ)
                    return 0;//在左
                else if (y < efishZ)
                    return 2;//在右
                else
                    return 1;//在其方向上
            }
            else
            {
                if (y > efishZ)
                    return 2;//在右
                else if (y < efishZ)
                    return 0;//在左
                else
                    return 1;//在其方向上
            }
        }
        #endregion


        #region 如果躲避鱼在攻击鱼左方就往左，右就往右
        void gogogo(Mission mission, ref Decision[] fish, int i, int teamId)
        {
            if (ejudge(mission, ref decisions, i, teamId) == 1)
            {
                if (judgeLorR(mission, ref decisions, i, teamId) == 0)
                {
                    decisions[i].TCode = 3;
                    decisions[i].VCode = 12;
                }
                if (judgeLorR(mission, ref decisions, i, teamId) == 1)
                {
                    decisions[i].TCode = 7;
                    decisions[i].VCode = 14;
                }
                if (judgeLorR(mission, ref decisions, i, teamId) == 2)
                {
                    decisions[i].TCode = 11;
                    decisions[i].VCode = 12;
                }
            }
        }
        #endregion


        #region 此函数可判断是否在指定时间段内   by闯
        /// <summary>
        /// 此函数可判断是否在指定时间段内
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="fish"></param>
        /// <param name="t1"></param>时间段开始时间（秒）
        /// <param name="t2"></param>时间段结束时间（秒）
        /// <returns></returns>
        bool TtoT(Mission mission, ref Decision[] fish, int t1, int t2)
        {
            if (mission.CommonPara.RemainingCycles < 6000 - t1 * 10 && mission.CommonPara.RemainingCycles > 6000 - t2 * 10)
                return true;
            else
                return false;
        }
        #endregion


        #region 该函数返回撞鱼点X坐标    
        /// <summary>
        /// 该函数返回撞球点X坐标    by闯
        /// </summary>
        /// <param name="cur_x"></param>当前鱼的X坐标
        /// <param name="cur_z"></param>当前鱼的Z坐标
        /// <param name="dest_x"></param>目标点的X坐标
        /// <param name="dest_z"></param>目标点的Z坐标
        /// <returns></returns>
        float GetPointX(float cur_x, float cur_z, float dest_x, float dest_z)
        {
            float x = 0;
            float a = 0;
            a = (float)(Math.Atan(Math.Abs(cur_z - dest_z) / Math.Abs(cur_x - dest_x)));
            if (cur_x != dest_x)
                x = (float)(cur_x * Math.Cos(a));
           // else if (cur_x < dest_x)
               // x = (float)(cur_x * Math.Cos(a));
            else
                x = cur_x;
            return x;
        }
        #endregion


        #region 该函数返回撞鱼点Z坐标    
        /// <summary>
        /// 该函数返回撞球点Z坐标    by闯
        /// </summary>
        /// <param name="cur_x"></param>当前鱼的X坐标
        /// <param name="cur_z"></param>当前鱼的Z坐标
        /// <param name="dest_x"></param>目标点的X坐标
        /// <param name="dest_z"></param>目标点的Z坐标
        /// <returns></returns>
        float GetPointZ(float cur_x, float cur_z, float dest_x, float dest_z)
        {
            float z = 0;
            float a = 0;
            a = (float)(Math.Atan(Math.Abs(cur_z - dest_z) / Math.Abs(cur_x - dest_x)));
            if (cur_z != dest_z)
                z = (float)(cur_z  * Math.Sin(a));
            //else if (cur_z > dest_z)
               // z = (float)(cur_z  * Math.Sin(a));
            else
                z = cur_z;
            return z;
        }
        #endregion


        #region 撞鱼函数
        void Rush(Mission mission, ref Decision[] fish, int i, int j, float dest_x, float dest_z, int teamId)
        {
            float a, b;
            xna.Vector3 destPtMm;
            destPtMm.X = GetPointX(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[j].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[j].PositionMm.Z, dest_x, dest_z);
            destPtMm.Y = 0;
            destPtMm.Z = GetPointZ(mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[j].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[j].PositionMm.Z, dest_x, dest_z);
            a = GetAngleDegree(destPtMm);
            b = mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad;
            if (Math.Abs(a - b) > 5 && Math.Abs(a - b) <= 8)
            {
                StrategyHelper.Helpers.Dribble(ref decisions[i], mission.TeamsRef[teamId].Fishes[i], destPtMm, a, 6, 14, 20, 15, 15, 21, 50, false);
            }
            if (Math.Abs(a - b) > 8 && Math.Abs(a - b) <= 12)
            {
                StrategyHelper.Helpers.Dribble(ref decisions[i], mission.TeamsRef[teamId].Fishes[i], destPtMm, a, 10, 14, 30, 15,15, 16, 50, false);
            }
            else if (Math.Abs(a - b) > 12 && Math.Abs(a - b) <= 16)
            {
                StrategyHelper.Helpers.Dribble(ref decisions[i], mission.TeamsRef[teamId].Fishes[i], destPtMm, a, 14, 14, 40, 15, 15, 12, 60, false);
            }
            else if (Math.Abs(a - b) > 16 && Math.Abs(a - b) <= 20)
            {
                StrategyHelper.Helpers.Dribble(ref decisions[i], mission.TeamsRef[teamId].Fishes[i], destPtMm, a, 18, 14, 50, 15, 15, 9, 70, false);
            }
            else if (Math.Abs(a - b) > 20)
            {
                StrategyHelper.Helpers.Dribble(ref decisions[i], mission.TeamsRef[teamId].Fishes[i], destPtMm, a, 24, 14, 60, 15, 15, 8, 80, false);
            }
            else
            {
                StrategyHelper.Helpers.Dribble(ref decisions[i], mission.TeamsRef[teamId].Fishes[i], destPtMm, a, 4, 14, 60, 15, 15, 16, 100, false);
            }
            return;

        }

        #endregion

        int TransfromAngletoTCode(float angvel)
        {
            if (0 == angvel)
            {
                return 7;
            }
            else if (angvel < 0)
            {
                if (-0.005395 <= angvel && 0 > angvel)
                {
                    if ((0 - angvel) >= (angvel + 0.005395))
                        return 6;
                    else
                        return 7;

                }
                else if (-0.009016 <= angvel && -0.005395 > angvel)
                {
                    if ((-0.005395 - angvel) >= (angvel + 0.009016))
                        return 5;
                    else
                        return 6;

                }
                else if (-0.014203 <= angvel && -0.009016 > angvel)
                {
                    if ((-0.009016 - angvel) >= (angvel + 0.014203))
                        return 4;
                    else
                        return 5;
                }
                else if (-0.019907 <= angvel && -0.014203 > angvel)
                {
                    if ((-0.014203 - angvel) >= (angvel + 0.019907))
                        return 3;
                    else
                        return 4;
                }
                else if (-0.0253 <= angvel && -0.019907 > angvel)
                {
                    if ((-0.019907 - angvel) >= (angvel + 0.0253))
                        return 2;
                    else
                        return 3;
                }
                else if (-0.033592 <= angvel && -0.0253 > angvel)
                {
                    if ((-0.0253 - angvel) >= (angvel + 0.033592))
                        return 1;
                    else
                        return 2;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (0.005395 >= angvel && 0 < angvel)
                {
                    if (angvel - 0 > 0.005395 - angvel)
                        return 8;
                    else
                        return 7;

                }
                else if (0.009016 >= angvel && 0.005395 < angvel)
                {
                    if (angvel - 0.005395 > 0.009016 - angvel)
                        return 9;
                    else
                        return 8;
                }
                else if (0.014203 >= angvel && 0.009016 < angvel)
                {
                    if (angvel - 0.009016 > 0.014203 - angvel)
                        return 10;
                    else
                        return 9;
                }
                else if (0.019907 >= angvel && 0.014203 < angvel)
                {
                    if (angvel - 0.014203 > 0.019907 - angvel)
                        return 11;
                    else
                        return 10;
                }
                else if (0.0253 >= angvel && 0.019907 < angvel)
                {
                    if (angvel - 0.019907 > 0.0253 - angvel)
                        return 12;
                    else
                        return 11;
                }
                else if (0.033592 >= angvel && 0.0253 < angvel)
                {
                    if (angvel - 0.0253 > 0.033592 - angvel)
                        return 13;
                    else
                        return 12;
                }
                else if (0.040848 >= angvel && 0.033592 < angvel)
                {
                    if (angvel - 0.033592 > 0.040848 - angvel)
                        return 14;
                    else
                        return 13;
                }
                else
                {
                    return 14;
                }

            }
        }

        # region 优先级选定

        #endregion


        /// <summary>
        /// 获取当前仿真使命（比赛项目）当前队伍所有仿真机器鱼的决策数据构成的数组
        /// </summary>
        /// <param name="mission">服务端当前运行着的仿真使命Mission对象</param>
        /// <param name="teamId">当前队伍在服务端运行着的仿真使命中所处的编号 
        /// 用于作为索引访问Mission对象的TeamsRef队伍列表中代表当前队伍的元素</param>
        /// <returns>当前队伍所有仿真机器鱼的决策数据构成的Decision数组对象</returns>
        public Decision[] GetDecision(Mission mission, int teamId)
        {
            // 决策类当前对象第一次调用GetDecision时Decision数组引用为null
            if (decisions == null)
            {// 根据决策类当前对象对应的仿真使命参与队伍仿真机器鱼的数量分配决策数组空间
                decisions = new Decision[mission.CommonPara.FishCntPerTeam];
            }

            #region 决策计算过程 需要各参赛队伍实现的部分
            #region 策略编写帮助信息
            //====================我是华丽的分割线====================//
            //======================策略编写指南======================//
            //1.策略编写工作直接目标是给当前队伍决策数组decisions各元素填充决策值
            //2.决策数据类型包括两个int成员，VCode为速度档位值，TCode为转弯档位值
            //3.VCode取值范围0-14共15个整数值，每个整数对应一个速度值，速度值整体但非严格递增
            //有个别档位值对应的速度值低于比它小的档位值对应的速度值，速度值数据来源于实验
            //4.TCode取值范围0-14共15个整数值，每个整数对应一个角速度值
            //整数7对应直游，角速度值为0，整数6-0，8-14分别对应左转和右转，偏离7越远，角度速度值越大
            //5.任意两个速度/转弯档位之间切换，都需要若干个仿真周期，才能达到稳态速度/角速度值
            //目前运动学计算过程决定稳态速度/角速度值接近但小于目标档位对应的速度/角速度值
            //6.决策类Strategy的实例在加载完毕后一直存在于内存中，可以自定义私有成员变量保存必要信息
            //但需要注意的是，保存的信息在中途更换策略时将会丢失
            //====================我是华丽的分割线====================//
            //=======策略中可以使用的比赛环境信息和过程信息说明=======//
            //场地坐标系: 以毫米为单位，矩形场地中心为原点，向右为正X，向下为正Z
            //            负X轴顺时针转回负X轴角度范围为(-PI,PI)的坐标系，也称为世界坐标系
            //mission.CommonPara: 当前仿真使命公共参数
            //mission.CommonPara.FishCntPerTeam: 每支队伍仿真机器鱼数量
            //mission.CommonPara.MsPerCycle: 仿真周期毫秒数
            //mission.CommonPara.RemainingCycles: 当前剩余仿真周期数
            //mission.CommonPara.TeamCount: 当前仿真使命参与队伍数量
            //mission.CommonPara.TotalSeconds: 当前仿真使命运行时间秒数
            //mission.EnvRef.Balls: 
            //当前仿真使命涉及到的仿真水球列表，列表元素的成员意义参见URWPGSim2D.Common.Ball类定义中的注释
            //mission.EnvRef.FieldInfo: 
            //当前仿真使命涉及到的仿真场地，各成员意义参见URWPGSim2D.Common.Field类定义中的注释
            //mission.EnvRef.ObstaclesRect: 
            //当前仿真使命涉及到的方形障碍物列表，列表元素的成员意义参见URWPGSim2D.Common.RectangularObstacle类定义中的注释
            //mission.EnvRef.ObstaclesRound:
            //当前仿真使命涉及到的圆形障碍物列表，列表元素的成员意义参见URWPGSim2D.Common.RoundedObstacle类定义中的注释
            //mission.TeamsRef[teamId]:
            //决策类当前对象对应的仿真使命参与队伍（当前队伍）
            //mission.TeamsRef[teamId].Para:
            //当前队伍公共参数，各成员意义参见URWPGSim2D.Common.TeamCommonPara类定义中的注释
            //mission.TeamsRef[teamId].Fishes:
            //当前队伍仿真机器鱼列表，列表元素的成员意义参见URWPGSim2D.Common.RoboFish类定义中的注释
            //mission.TeamsRef[teamId].Fishes[i].PositionMm和PolygonVertices[0],BodyDirectionRad,VelocityMmPs,
            //                                   AngularVelocityRadPs,Tactic:
            //当前队伍第i条仿真机器鱼鱼体矩形中心和鱼头顶点在场地坐标系中的位置（用到X坐标和Z坐标），鱼体方向，速度值，
            //                                   角速度值，决策值
            //====================我是华丽的分割线====================//
            //========================典型循环========================//
            //for (int i = 0; i < mission.CommonPara.FishCntPerTeam; i++)
            //{
            //  decisions[i].VCode = 0; // 静止
            //  decisions[i].TCode = 7; // 直游
            //}
            //====================我是华丽的分割线====================//
            #endregion
            //请从这里开始编写代码

            #region 简化变量及标准量定义
            int by2 = Convert.ToInt32(mission.HtMissionVariables["IsYellowFish2Caught"]);
            int by3 = Convert.ToInt32(mission.HtMissionVariables["IsYellowFish3Caught"]);
            int by4 = Convert.ToInt32(mission.HtMissionVariables["IsYellowFish4Caught"]);
            int br2 = Convert.ToInt32(mission.HtMissionVariables["IsRedFish2Caught"]);
            int br3 = Convert.ToInt32(mission.HtMissionVariables["IsRedFish3Caught"]);
            int br4 = Convert.ToInt32(mission.HtMissionVariables["IsRedFish4Caught"]);
            int flag=0;
            int[] b = new int[3];
            float dir1 = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad,
                 dir2 = mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad,
                 dir3 = mission.TeamsRef[teamId].Fishes[2].BodyDirectionRad,
                 dir4 = mission.TeamsRef[teamId].Fishes[3].BodyDirectionRad,
                 edir1 = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].BodyDirectionRad,
                 edir2 = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].BodyDirectionRad,
                 edir3 = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].BodyDirectionRad,
                 edir4 = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].BodyDirectionRad,
                 fish0X = mission.TeamsRef[teamId].Fishes[0].PositionMm.X,
                 fish1X = mission.TeamsRef[teamId].Fishes[1].PositionMm.X,
                 fish2X = mission.TeamsRef[teamId].Fishes[2].PositionMm.X,
                 fish3X = mission.TeamsRef[teamId].Fishes[3].PositionMm.X,
                 fish0Z = mission.TeamsRef[teamId].Fishes[0].PositionMm.Z,
                 fish1Z = mission.TeamsRef[teamId].Fishes[1].PositionMm.Z,
                 fish2Z = mission.TeamsRef[teamId].Fishes[2].PositionMm.Z,
                 fish3Z = mission.TeamsRef[teamId].Fishes[3].PositionMm.Z,
                 efish0X = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X,
                 efish1X = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].PositionMm.X,
                 efish2X = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].PositionMm.X,
                 efish3X = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].PositionMm.X,
                 efish0Z = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z,
                 efish1Z = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[1].PositionMm.Z,
                 efish2Z = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[2].PositionMm.Z,
                 efish3Z = mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[3].PositionMm.Z;
            #endregion
            //**********************************************//
            #region 进攻方
           
            

             if (mission.TeamsRef[teamId].Para.MyHalfCourt == 0)
             {
                 if (TtoT(mission, ref decisions, 0, 300)==true)
                 {
                     b[0] = by2;
                     b[1] = by3;
                     b[2] = by4;
                 }
                 else
                 {
                     b[0] = br2;
                     b[1] = br3;
                     b[2] = br4;
                 }

                int i = minlength(mission, ref decisions, b[0], b[1], b[2], teamId);
                float efish_iX,efish_iZ;
                efish_iX = mission.TeamsRef[teamId].Fishes[i].PositionMm.X;
                efish_iZ = mission.TeamsRef[teamId].Fishes[i].PositionMm.Z;
                int judge=0;
                judge=Judge_the_same(mission, ref decisions, fish0X, efish1X, efish2X, efish3X);//判断是否至少有一条鱼和我方进攻鱼在同一半区,如果有返回一，如果无返回0
                if (judge)
                {
                    if (efish_iX >= 300 || efish_iX <= -300)
                    {
                        attack1(mission, ref decisions, efish_iX, efish_iZ);//此函数实现在空旷地方进攻对方躲避鱼
                    }
                    else
                    {
                        attack2(mission, ref decisions, efish_iX, efish_iZ)//此函数实现在方形障碍物附近进行捕鱼
                    }

                }
                else//选择穿过的孔
                {
                    int hole_z;
                    hole_z=switch_hole(mission, ref decisions,fish0X,fish0Z);//此函数通过判断我方鱼所处区域以及方向选择4个孔之中合适的孔穿过返回孔的z坐标
                    
                    SwimToDest(mission, ref decisions,0, hole_z);
                    SwimToDest(mission, ref decisions, -100, hole_z);
                }
                //touch2(mission, ref decisions, i, teamId);
                //SSwimToDest(mission, ref decisions, 0, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[i].PolygonVertices[0].X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[i].PolygonVertices[0].Z, 0, teamId);
                //if(TtoT(mission, ref decisions, 0, 20) == true)
                //{

                //}


            }
            #endregion
            //**********************************************//
            #region 防守方
            else
            {
                if (TtoT(mission, ref decisions, 0, 300) == true)
                {
                    b[0] = by2;
                    b[1] = by3;
                    b[2] = by4;
                }
                else
                {
                    b[0] = br2;
                    b[1] = br3;
                    b[2] = br4;
                }
                float r = 350, d = 50, R;
                float l1 = GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[1].PositionMm.X, mission.TeamsRef[teamId].Fishes[1].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z),
                      l2 = GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[2].PositionMm.X, mission.TeamsRef[teamId].Fishes[2].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z),
                      l3 = GetLengthToDestpoint(mission.TeamsRef[teamId].Fishes[3].PositionMm.X, mission.TeamsRef[teamId].Fishes[3].PositionMm.Z, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.X, mission.TeamsRef[Math.Abs(teamId - 1)].Fishes[0].PositionMm.Z);
                //********************************************//
                #region 1号鱼
                
                

               
                #endregion
                //********************************************//
                #region 2号鱼
               
               
                #endregion
                //********************************************//
                #region 3号鱼
                #region 2,4号鱼都没下场
               
                #endregion
                /////////////////////////////////////////////
                #region 2号鱼下场4号鱼不下场
               
                #endregion
                //////////////////////////////////////////////////
                #region 4号鱼下场2号鱼没下场
              
                #endregion
                //////////////////////////////////////////////////
                #region 2,4号鱼都已经下场
              
                #endregion
                #endregion
                //********************************************//
             
                #region 4号鱼
                 
                  #endregion
            }
            #endregion
            #endregion

            return decisions;
        }
    }
}
