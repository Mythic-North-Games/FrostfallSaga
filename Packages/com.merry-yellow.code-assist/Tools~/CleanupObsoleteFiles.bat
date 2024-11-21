:: removes obsolete files left from previous versions



:: remove ProjectPath/Assets/Plugins/CodeAssist/Editor/ExternalReferences/Release/netstandard2.0/ directory files
:: with version UCA.v.1.1.9 and newer versions, they are located at ProjectPath/Assets/Plugins/CodeAssist/Editor/ExternalReferences
SET @filepath=%~dp0..\..\..\Assets\Plugins\CodeAssist\Editor\ExternalReferences\Release\netstandard2.0\
CALL :DeleteFileAndItsMeta "UnityCodeAssistSynchronizerModel.deps.json"
CALL :DeleteFileAndItsMeta "UnityCodeAssistSynchronizerModel.dll"
CALL :DeleteFileAndItsMeta "UnityCodeAssistSynchronizerModel.pdb"
CALL :DeleteFileAndItsMeta "UnityCodeAssistYamlDotNet.deps.json"
CALL :DeleteFileAndItsMeta "UnityCodeAssistYamlDotNet.dll"
CALL :DeleteFileAndItsMeta "UnityCodeAssistYamlDotNet.pdb"
CALL :DeleteFileAndItsMeta "UnityCodeAssistYamlDotNet.xml"

SET @filepath=%~dp0..\..\..\Assets\Plugins\CodeAssist\Editor\ExternalReferences\Release\netstandard2.0
CALL :DeleteFolderAndItsMeta
SET @filepath=%~dp0..\..\..\Assets\Plugins\CodeAssist\Editor\ExternalReferences\Release
CALL :DeleteFolderAndItsMeta



:: remove non-customized binary files
:: with version UCA.v.1.1.12, dll files has been customized (ex. AsyncIO.dll is now Meryel.UnityCodeAssist.AsyncIO.dll)
SET @filepath=%~dp0..\..\..\Assets\Plugins\CodeAssist\Editor\ExternalReferences\
CALL :DeleteFileAndItsMeta "AsyncIO.dll"
CALL :DeleteFileAndItsMeta "NaCl.dll"
CALL :DeleteFileAndItsMeta "NetMQ.dll"
CALL :DeleteFileAndItsMeta "Serilog.dll"
CALL :DeleteFileAndItsMeta "Serilog.Sinks.PersistentFile.dll"



:: remove system binary files
:: with Unity 2021.2 and newer, they are not needed anymore
SET @filepath=%~dp0..\..\..\Assets\Plugins\CodeAssist\Editor\ExternalReferences\
CALL :DeleteFileAndItsMeta "System.Buffers.dll"
CALL :DeleteFileAndItsMeta "System.Memory.dll"
CALL :DeleteFileAndItsMeta "System.Runtime.CompilerServices.Unsafe.dll"
CALL :DeleteFileAndItsMeta "System.Threading.Tasks.Extensions.dll"



:: remove old assets directory
:: with version UCA.v.1.2, asset directory has been relocated to Packages from Assets
SET @filepath=%~dp0..\..\..\Assets\Plugins\CodeAssist\Editor\
CALL :DeleteFileAndItsMeta "TinyJson\JsonWriter.cs"
CALL :DeleteFileAndItsMeta "TinyJson\JsonParser.cs"
CALL :DeleteFileAndItsMeta "Preferences\RegistryMonitor.cs"
CALL :DeleteFileAndItsMeta "Preferences\PreferenceStorageAccessor.cs"
CALL :DeleteFileAndItsMeta "Preferences\PreferenceMonitor.cs"
CALL :DeleteFileAndItsMeta "Preferences\PreferenceEntryHolder.cs"
CALL :DeleteFileAndItsMeta "Logger\UnitySink.cs"
CALL :DeleteFileAndItsMeta "Logger\MemorySink.cs"
CALL :DeleteFileAndItsMeta "Logger\ELogger.cs"
CALL :DeleteFileAndItsMeta "Logger\DomainHashEnricher.cs"
CALL :DeleteFileAndItsMeta "Logger\CommonTools.cs"
CALL :DeleteFileAndItsMeta "Logger\Attributes.cs"
CALL :DeleteFileAndItsMeta "Input\UnityInputManager.cs"
CALL :DeleteFileAndItsMeta "Input\Text2Yaml.cs"
CALL :DeleteFileAndItsMeta "Input\InputManagerMonitor.cs"
CALL :DeleteFileAndItsMeta "Input\Binary2TextExec.cs"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.YamlDotNet.xml"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.YamlDotNet.pdb"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.YamlDotNet.dll"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.YamlDotNet.deps.json"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.SynchronizerModel.pdb"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.SynchronizerModel.dll"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.SynchronizerModel.deps.json"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.Serilog.xml"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.Serilog.Sinks.PersistentFile.pdb"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.Serilog.Sinks.PersistentFile.dll"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.Serilog.Sinks.PersistentFile.deps.json"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.Serilog.pdb"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.Serilog.dll"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.NetMQ.xml"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.NetMQ.pdb"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.NetMQ.dll"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.NetMQ.deps.json"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.NaCl.xml"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.NaCl.pdb"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.NaCl.dll"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.AsyncIO.pdb"
CALL :DeleteFileAndItsMeta "ExternalReferences\Meryel.UnityCodeAssist.AsyncIO.dll"
CALL :DeleteFileAndItsMeta "EditorCoroutines\EditorWindowCoroutineExtension.cs"
CALL :DeleteFileAndItsMeta "EditorCoroutines\EditorWaitForSeconds.cs"
CALL :DeleteFileAndItsMeta "EditorCoroutines\EditorCoroutineUtility.cs"
CALL :DeleteFileAndItsMeta "EditorCoroutines\EditorCoroutine.cs"
CALL :DeleteFileAndItsMeta "UnityClassExtensions.cs"
CALL :DeleteFileAndItsMeta "StatusWindow.cs"
CALL :DeleteFileAndItsMeta "ScriptFinder.cs"
CALL :DeleteFileAndItsMeta "NetMQPublisher.cs"
CALL :DeleteFileAndItsMeta "NetMQInitializer.cs"
CALL :DeleteFileAndItsMeta "Monitor.cs"
CALL :DeleteFileAndItsMeta "MerryYellow.CodeAssist.Editor.asmdef"
CALL :DeleteFileAndItsMeta "MainThreadDispatcher.cs"
CALL :DeleteFileAndItsMeta "LazyInitializer.cs"
CALL :DeleteFileAndItsMeta "FeedbackWindow.cs"
CALL :DeleteFileAndItsMeta "Assister.cs"
CALL :DeleteFileAndItsMeta "AboutWindow.cs"
::CALL :DeleteFileAndItsMeta "TinyJson"
::CALL :DeleteFileAndItsMeta "Preferences"
::CALL :DeleteFileAndItsMeta "Logger"
::CALL :DeleteFileAndItsMeta "Input"
::CALL :DeleteFileAndItsMeta "ExternalReferences"
::CALL :DeleteFileAndItsMeta "EditorCoroutines"



EXIT /B 0



:DeleteFileAndItsMeta
::ECHO Deleting "%@filepath%%~1"
DEL "%@filepath%%~1" /F
DEL "%@filepath%%~1.meta" /F
EXIT /B 0



:DeleteFolderAndItsMeta
rd "%@filepath%" /Q
del "%@filepath%.meta" /F
EXIT /B 0
