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

using System.ComponentModel;
using BH.oM.Base;
using System.Collections.Generic;

namespace BH.oM.Materials2050.Fragments
{
    [Description("A data fragment containing API-specific values to help navigate paginated results from the GET request.")]
    public class ResultsNavigation : IFragment
    {
        [Description("The API used for the GET request.")]
        public virtual APIName APIName { get; set; } = APIName.Undefined;

        [Description("Total number of products received from the GET request.")]
        public virtual int TotalProducts { get; set; } = 0;

        [Description("Number of products on the current page of the results.")]
        public virtual int ProductsOnPage { get; set; } = 0;

        [Description("The current page number.")]
        public virtual int CurrentPage { get; set; } = 0;

        [Description("The indexed products currently being viewed.")]
        public virtual string ProductRange { get; set; } = null;
    }
}




