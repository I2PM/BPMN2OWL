## MasterThesis Tim Tobias Braunauer (BPMN to PASS translation of business process models)

This project can be used to convert BPMN models into PASS models.
Not all BPMN models can be translated, but only those modeled using the modeling guide created in my master thesis.

In order to use the software, the following folder structure must first be created:

* BPMN
  * Input
  * Clean
  * Reverse
* PASS
  * Output

After that, the path to the folder structure must be specified in the `Program.cs` file.
All BPMN files that are to be translated must then be placed in the `BPMN/Input` folder.

After that, 3 translation steps are performed.

1. the software exports all files from this folder ending with `.bpmn` when run, cleans them from unnecessary data and saves them under `BPMN/Clean`.
2. these files are imported again and converted into PASS models. They are then exported to the `PASS/Output` folder using the [alps.net.api](https://github.com/I2PM/alps.net.api "alps.net.api") library.
3. the PASS model is translated back to a BPMN model and saved as `BPMN/Reverse`. This allows to compare if important data has been lost during translation.

The translated PASS OWL file can be executed afterwards.
