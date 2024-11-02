//=============================================================================
// This file is part of The Scripps Research Institute's C-ME Application built
// by InterKnowlogy.  
//
// Copyright (C) 2006, 2007 Scripps Research Institute / InterKnowlogy, LLC.
// All rights reserved.
//
// For information about this application contact Tim Huckaby at
// TimHuck@InterKnowlogy.com or (760) 930-0075 x201.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//=============================================================================

using System.Collections.Generic;
using UnityEngine;


namespace MoleculeLogic
{
    /// <summary>
    /// Abstract base class for objects that support changing their visual state when hovered and
    /// displaying <see cref="HoverPopup" /> windows.
    /// </summary>
    public class HoverObject: MonoBehaviour
    {


    }

    public class Representable : HoverObject
    {
        public virtual void Represent(MoleculeScheme scheme, MolType type, bool dirty, int level)
        { }

    }
}
