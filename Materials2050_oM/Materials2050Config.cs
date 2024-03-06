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

using BH.oM.Adapter;
using System;
using System.ComponentModel;
using BH.oM.Materials2050;

namespace BH.oM.Adapters.Materials2050
{
    [Description("This Config can be specified in the `ActionConfig` input of any Adapter Action (e.g. Push).")]
    // Note: this will get passed within any CRUD method (see their signature). 

    public class Materials2050Config : ActionConfig
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        [Description("Please specify the API you would like to query. OpenAPI is the default option and is free to use (rate limited). All other options require subscriptions. More information can be found on 2050 Materials Website.")]
        public virtual APIName APIName { get; set; } = APIName.OpenAPI;

        [Description("All results from the 2050 Materials API are paginated to reduce API call count.")]
        public virtual int Page { get; set; } = 1;

        [Description("Specifies string to search and return objects for in 2050 Materials, ie RedBuilt RedLam LVL.")]
        public virtual string NameLike { get; set; } = null;

        // Make this an enum when types can be identified by the 2050 Materials group. Int associations are currently unknown, but still function as api calls. 
        [Description("Setting this will return materials of a specific type. Types can be found on 2050 Materials Documentation.")]
        public virtual int? MaterialType { get; set; } = null;

        [Description("Sort all results by a specified sort by option.")]
        public virtual SortBy SortBy { get; set; } = SortBy.Undefined;

        [Description("Group all results by a specified group by option.")]
        public virtual GroupBy GroupBy { get; set; } = GroupBy.Undefined;

        /***************************************************/
    }
}