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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.oM.Adapters.SoftwareName
{
    [Description("Object description in here. Will appear in the UI tooltip.")]
    public class ExampleObject : BHoMObject
    {
        // // See examples in the BHoM repo and the wiki to see how we define types.
        // // Generally, all properties should be public and have public getter and setter.
        // // BHoM Objects should have orthogonal properties and no behaviour (no methods), as in C# Records (or Python Dataclasses).
        // // No constructor should be specified. If a specific instantiaton method is needed, we make it as an "Engine/Create" method.
        // // Objects created with this convention will automatically appear as UI components (e.g. Grasshopper component).

        [Description("Property description in here.")]
        public string SomeStringProperty { get; set; }

        [Description("Property description in here.")]
        public int SomeNumberProperty { get; set; }
    }
}


