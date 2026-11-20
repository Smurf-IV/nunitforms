# NUnit.WinForms 🌟

## Who

- V1.x Copyright (c) 2003-2006 by Luke T. Maxon
- V2.x Copyright (c) 2006-2010 by Luke T. Maxon and others
- V3.x Copyright (c) 2026-onwards by Smurf-IV

## When

- As the timeline shows above, V1 -> V2 was a long time ago and had nuget names like `NUnitForms` and `NUnitforms.Framework`.
- V3 starts the `NUnit.WinForms` nuget package based on the above.

## Why

- I wanted a way to be able to test standalone controls (Actually for wizard pages),
 but could not find a way to do this without having to perform reflection on the `Focus_Lost` events, because of the way Dev-Ops runners work (i.e. headless)
- I found this (I had used it in 2006 for a couple of projects) and thought it would be a good idea to modernize it and make it work with the latest NUnit TFM(s) and also fix / add some new features.

## What

- It will be a fork of the original NUnitForms project, but with a new name and a new nuget package name.
- Support for the latest NUnit TFM(s) and also fix / add some new features.

-----