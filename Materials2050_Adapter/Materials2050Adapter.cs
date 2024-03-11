/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.Adapter;
using System.Net;
using System.Net.Http;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.Engine.Adapters.Materials2050;

namespace BH.Adapter.Materials2050
{
    public partial class Materials2050Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("Adapter to connect to 2050 Materials API.")]
        [Input("apiToken", "Provide 2050 Materials API Token")]
        [Output("adapter", "Adapter results")]
        public Materials2050Adapter(string apiToken = "", bool active = false)
        {
            if (active)
            {
                m_apiToken = apiToken;
            }
        }

        /***************************************************/
        /*** Private Fields                              ***/
        /***************************************************/

        private static string m_apiToken = null;

    }
}


