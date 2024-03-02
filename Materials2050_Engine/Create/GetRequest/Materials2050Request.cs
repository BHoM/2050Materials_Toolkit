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
using BH.oM.Adapters.HTTP;
using BH.oM.Adapters.Materials2050;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Adapters.Materials2050
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public  Method                            ****/
        /***************************************************/

        [Description("Create a GetRequest for the 2050 Materials")]
        [Input("apiCommand", "The 2050 Materials API command to create a GetRequest with")]
        [Input("apiToken", "The user's 2050 Materials APIToken")]
        [Input("parameters", "An optional config object with properties representing parameters to create the GetRequest with (ie count, name_like, etc)")]
        [Output("GetRequest", "A GetRequest with 2050 Materials specific headers and uri")]
        public static GetRequest Materials2050Request(string apiCommand, string apiToken, Materials2050Config parameters = null)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();

            if (parameters.Page > 1)
                param.Add("page", parameters.Page);

            if (parameters.NameLike != null && parameters.NameLike != "")
                param.Add("name", parameters.NameLike);

            if(parameters.MaterialType != null && parameters.MaterialType >= 0)
                param.Add("material_types" , parameters.MaterialType);

            if (parameters.SortBy != SortBy.Undefined)
                param.Add("sort_by", parameters.SortBy);

            if (parameters.GroupBy != GroupBy.Undefined)
                param.Add("group_by", parameters.GroupBy);

            return new BH.oM.Adapters.HTTP.GetRequest
            {
                BaseUrl = "https://app.2050-materials.com/developer/api/" + apiCommand,
                Headers = new Dictionary<string, object>()
                {
                    { "Authorization", "Bearer " + apiToken }
                },
                Parameters = param
            };
        }
        /***************************************************/
    }
}



