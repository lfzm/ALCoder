using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AL.Framework.WebMvc
{
    /// <summary>
    /// 注册区域控制器
    /// </summary>
    public abstract class BaseAreaRegistration : AreaRegistration
    {
        protected static List<AreaRegistrationContext> areaContent = new List<AreaRegistrationContext>();
        protected static List<BaseAreaRegistration> areaRegistration = new List<BaseAreaRegistration>();


        /// <summary>
        /// 注册方法
        /// </summary>
        private static void Register()
        {
            List<int[]> list = new List<int[]>();
            for (int i = 0; i < areaRegistration.Count; i++)
            {
                list.Add(new int[] { areaRegistration[i].Order, i });
            }
            foreach (int[] numArray in (from o in list
                                        orderby o[0]
                                        select o).ToList<int[]>())
            {
                areaRegistration[numArray[1]].RegisterAreaOrder(areaContent[numArray[1]]);
            }
        }


        public static void RegisterAreas()
        {
            AreaRegistration.RegisterAllAreas();
            Register();
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            areaContent.Add(context);
            areaRegistration.Add(this);
        }

        public abstract void RegisterAreaOrder(AreaRegistrationContext context);

        public abstract int Order { get; }
    }
}
