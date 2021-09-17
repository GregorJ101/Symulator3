using System;                     // EventArgs, DateTime, EventHandler
using System.Collections.Generic; // List, Dictionary
using System.IO;                  // File
using System.Threading.Tasks;     // Task, Task<>
using System.Text;                // StringBuilder
using System.Threading;           // Thread
using System.Windows;

namespace EmulatorEngine
{
    #region EventArgs definitions
    public class NewIAREventArgs : EventArgs
    {
        public NewIAREventArgs (int iNewIAR, int iNewIL)
        {
            NewIAR = iNewIAR;
            NewIL  = iNewIL;
        }
        public int NewIAR { get; set; }
        public int NewIL  { get; set; }
    }

    public class NewARREventArgs : EventArgs
    {
        public NewARREventArgs (int iNewARR, int iNewIL)
        {
            NewARR = iNewARR;
            NewIL  = iNewIL;
        }
        public int NewARR { get; set; }
        public int NewIL  { get; set; }
    }

    public class NewXR1EventArgs : EventArgs
    {
        public NewXR1EventArgs (int iNewXR1)
        { NewXR1 = iNewXR1; }
        public int NewXR1 { get; set; }
    }

    public class NewXR2EventArgs : EventArgs
    {
        public NewXR2EventArgs (int iNewXR2)
        { NewXR2 = iNewXR2; }
        public int NewXR2 { get; set; }
    }

    public class NewCREventArgs : EventArgs
    {
        public NewCREventArgs (byte yNewCR, bool bSystemReset = false)
        {
            NewCR       = yNewCR;
            SystemReset = bSystemReset;
        }
        public byte NewCR { get; set; }
        public bool SystemReset { get; set; }
    }

    public class NewCPUDialsEventArgs : EventArgs
    {
        public NewCPUDialsEventArgs (Int16 iNewCPUDials, bool bEnable = true)
        {
            NewCPUDials = iNewCPUDials;
            Enable      = bEnable;
        }
        public Int16 NewCPUDials { get; set; }
        public bool  Enable      { get; set; }
    }

    public class NewEnableHighlightLineEventArgs : EventArgs
    {
        public NewEnableHighlightLineEventArgs (bool bEnable = true)
        {
            Enable = bEnable;
        }
        public bool Enable { get; set; }
    }

    public class NewDisassemblyEventArgs : EventArgs
    {
        public NewDisassemblyEventArgs (int iBeginDasmAddress, int iEndDasmAddress, int iDasmEntryPoint, int iXR1, int iXR2)
        {
            BeginDasmAddress = iBeginDasmAddress;
            EndDasmAddress   = iEndDasmAddress;
            DasmEntryPoint   = iDasmEntryPoint;
            XR1              = iXR1;
            XR2              = iXR2;
        }
        public int BeginDasmAddress { get; set; }
        public int EndDasmAddress   { get; set; }
        public int DasmEntryPoint   { get; set; }
        public int XR1              { get; set; }
        public int XR2              { get; set; }
    }

    public class NewLPFLREventArgs : EventArgs
    {
        public NewLPFLREventArgs (int iNewLPFLR)
        { NewLPFLR = iNewLPFLR; }
        public int NewLPFLR { get; set; }
    }

    public class NewLPIAREventArgs : EventArgs
    {
        public NewLPIAREventArgs (int iNewLPIAR)
        { NewLPIAR = iNewLPIAR; }
        public int NewLPIAR { get; set; }
    }

    public class NewLPDAREventArgs : EventArgs
    {
        public NewLPDAREventArgs (int iNewLPDAR)
        { NewLPDAR = iNewLPDAR; }
        public int NewLPDAR { get; set; }
    }

    public class NewMPDAREventArgs : EventArgs
    {
        public NewMPDAREventArgs (int iNewMPDAR)
        { NewMPDAR = iNewMPDAR; }
        public int NewMPDAR { get; set; }
    }

    public class NewMRDAREventArgs : EventArgs
    {
        public NewMRDAREventArgs (int iNewMRDAR)
        { NewMRDAR = iNewMRDAR; }
        public int NewMRDAR { get; set; }
    }

    public class NewMUDAREventArgs : EventArgs
    {
        public NewMUDAREventArgs (int iNewMUDAR)
        { NewMUDAR = iNewMUDAR; }
        public int NewMUDAR { get; set; }
    }

    public class NewDCAREventArgs : EventArgs
    {
        public NewDCAREventArgs (int iNewDCAR)
        { NewDCAR = iNewDCAR; }
        public int NewDCAR { get; set; }
    }

    public class NewDRWAREventArgs : EventArgs
    {
        public NewDRWAREventArgs (int iNewDRWAR)
        { NewDRWAR = iNewDRWAR; }
        public int NewDRWAR { get; set; }
    }

    public class NewStepCountEventArgs : EventArgs
    {
        public NewStepCountEventArgs (int iNewStepCount)
        { NewStepCount = iNewStepCount; }
        public int NewStepCount { get; set; }
    }

    public class NewDASMStringEventArgs : EventArgs
    {
        public NewDASMStringEventArgs (string strNewDASMString)
        { NewDASMString = strNewDASMString; }
        public string NewDASMString { get; set; }
    }

    public class NewDASMStringListEventArgs : EventArgs
    {
        public NewDASMStringListEventArgs (List<string> lstrNewDASMStrings)
        { NewDASMStrings = lstrNewDASMStrings; }
        public List<string> NewDASMStrings { get; set; }
    }

    public class NewTraceStringEventArgs : EventArgs
    {
        public NewTraceStringEventArgs (string strNewTraceString)
        { NewTraceString = strNewTraceString; }
        public string NewTraceString { get; set; }
    }

    public class NewTraceStringListEventArgs : EventArgs
    {
        public NewTraceStringListEventArgs (List<string> lstrNewTraceStrings)
        { NewTraceStrings = lstrNewTraceStrings; }
        public List<string> NewTraceStrings { get; set; }
    }

    public class New5471StringEventArgs : EventArgs
    {
        public New5471StringEventArgs (string strNew5471String)
        { New5471String = strNew5471String; }
        public string New5471String { get; set; }
    }

    public class NewPrinterStringEventArgs : EventArgs
    {
        public NewPrinterStringEventArgs (string strNewPrinterString)
        { NewPrinterString = strNewPrinterString; }
        public string NewPrinterString { get; set; }
    }

    public class NewPrinterStringListEventArgs : EventArgs
    {
        public NewPrinterStringListEventArgs (List<string> lstrNewPrinterStrings)
        { NewPrinterStrings = lstrNewPrinterStrings; }
        public List<string> NewPrinterStrings { get; set; }
    }

    public class NewHaltCodeEventArgs : EventArgs
    {
        public NewHaltCodeEventArgs (string strNewHaltCode)
        { NewHaltCode = strNewHaltCode; }
        public string NewHaltCode { get; set; }
    }

    public class New5475CodeEventArgs : EventArgs
    {
        public New5475CodeEventArgs (string strNew5475Code)
        { New5475Code = strNew5475Code; }
        public string New5475Code { get; set; }
    }

    public class NewProgramStateEventArgs : EventArgs
    {
        public NewProgramStateEventArgs (string strNewProgramState)
        { NewProgramState = strNewProgramState; }
        public string NewProgramState { get; set; }
    }

    public class NewProcessorCheck1EventArgs : EventArgs
    {
        public NewProcessorCheck1EventArgs (string strProcessorCheck1)
        { ProcessorCheck1 = strProcessorCheck1; }
        public string ProcessorCheck1 { get; set; }
    }

    public class NewProcessorCheck2EventArgs : EventArgs
    {
        public NewProcessorCheck2EventArgs (string strProcessorCheck2)
        { ProcessorCheck2 = strProcessorCheck2; }
        public string ProcessorCheck2 { get; set; }
    }

    public class NewProgramSizeEventArgs : EventArgs
    {
        public NewProgramSizeEventArgs (int iProgramSize)
        { ProgramSize = iProgramSize; }
        public int ProgramSize { get; set; }
    }

    //public class ResetControlColorsEventArgs : EventArgs { }
    #endregion

    public class CEmulatorState
    {
        // Program states in emulator engine:

        // 1 NoProgramLoaded
        // 2 ProgramLoaded
        // 3 ProgramRunning (FreeRun, RunWithBreakPoint, SingleStep)
        // 4 ProgramPaused (BreakPoint, SingleStep)
        // 4 ProgramHalted (HPL, only when no in SingleStep; in SingleStep HPL is like any other instruction)
        // 5 ProgramStopped (ProcessorCheck)
        public enum EProgramState
        {
            PSTATE_Unspecified,
            PSTATE_1_NoProgramLoaded,                  // [x] PSTATE_2_ProgramLoaded
            PSTATE_2_ProgramLoaded,                    // [x] Any PSTATE_3_ values
            PSTATE_3_ProgramRunningFreeRun,            // [x] Any PSTATE_3_, PSTATE_4_, or PSTATE_5_ value
            PSTATE_3_ProgramRunningRunWithBreakPoints, // [x] Any PSTATE_3_, PSTATE_4_, or PSTATE_5_ value
            PSTATE_3_ProgramRunningSingleStep,         // [x] Any PSTATE_3_, PSTATE_4_, or PSTATE_5_ value
            //PSTATE_4_ProgramPausedBreakPoint,          // [x] Any PSTATE_3_ value
            //PSTATE_4_ProgramPausedSingleStep,          // [x] Any PSTATE_3_ value
            PSTATE_4_ProgramPaused,                    // [x] Any PSTATE_3_ value
            PSTATE_4_ProgramHalted,                    // [x] Any PSTATE_3_ value; state only valid if not in RUN_SingleStep
            PSTATE_5_Aborted,                          // [x] PSTATE_1_NoProgramLoaded
            PSTATE_5_PChk_InvalidOpCode,               // [x] PSTATE_1_NoProgramLoaded
            PSTATE_5_PChk_InvalidQByte,                // [x] PSTATE_1_NoProgramLoaded
            PSTATE_5_PChk_AddressWrap,                 // [x] PSTATE_1_NoProgramLoaded
            PSTATE_5_PChk_InvalidAddress,              // [x] PSTATE_1_NoProgramLoaded
            PSTATE_5_PChk_PL2_Unsupported,             // [x] PSTATE_1_NoProgramLoaded
            PSTATE_5_PChk_UnsupportedDevice            // [x] PSTATE_1_NoProgramLoaded
        };

        public enum ERunMode
        {
            RUN_NotSpecified,
            RUN_FreeRun,
            RUN_BreakPoints,
            RUN_SingleStep
        };

        public const string STATE_NOT_LOADED                            = "No Program Loaded";
        public const string STATE_LOADED                                = "Program Loaded";
        public const string PSTATE_PROGRAM_RUNNING_FREE_RUN             = "Program Running: Free Run";
        public const string PSTATE_PROGRAM_RUNNING_RUN_WITH_BREAK_POINT = "Program Running: Run With BreakPoints";
        public const string PSTATE_PROGRAM_RUNNING_SINGLE_STEP          = "Program Running: Single-Step";
        public const string PSTATE_PROGRAM_PAUSED_BREAKPOINT            = "Program Paused: BreakPoint";
        public const string PSTATE_PROGRAM_PAUSED_SINGLESTEP            = "Program Paused: SingleStep";
        public const string PSTATE_PROGRAM_PAUSED                       = "Program Paused";
        public const string PSTATE_PROGRAM_HALTED                       = "Program Halted";
        public const string PSTATE_PROGRAM_ABORTED                      = "Program Aborted";
        public const string PSTATE_PCHK_INVALID_OP_CODE                 = "Processor Check: Invalid OpCode";
        public const string PSTATE_PCHK_INVALID_Q_BYTE                  = "Processor Check: Invalid QByte";
        public const string PSTATE_PCHK_ADDRESS_WRAP                    = "Processor Check: Address Wrap";
        public const string PSTATE_PCHK_INVALID_ADDRESS                 = "Processor Check: Invalid Address";
        public const string PSTATE_PCHK_PL2_UNSUPPORTED                 = "Processor Check: PL2_Unsupported";
        public const string PSTATE_PCHK_UNSUPPORTED_DEVICE              = "Processor Check: Unsupported Device";
        public const string PSTATE_PROCESSOR_CHECK                      = "Processor Check";
        public const string PSTATE_PCHK_INVALID_OP_CODE_2               = "Invalid OpCode";
        public const string PSTATE_PCHK_INVALID_Q_BYTE_2                = "Invalid QByte";
        public const string PSTATE_PCHK_ADDRESS_WRAP_2                  = "Address Wrap";
        public const string PSTATE_PCHK_INVALID_ADDRESS_2               = "Invalid Address";
        public const string PSTATE_PCHK_PL2_UNSUPPORTED_2               = "PL2 Unsupported";
        public const string PSTATE_PCHK_UNSUPPORTED_DEVICE_2            = "Invalid Device";

        // Emulator menu options      Resulting EProgramState                    Resulting ERunMode
        // Emulator -> SystemReset    PSTATE_1_NoProgramLoaded                   RUN_NotSpecified
        // Emulator -> Run            PSTATE_3_ProgramRunningRunWithBreakPoints  RUN_BreakPoints
        // Emulator -> FreeRun        PSTATE_3_ProgramRunningFreeRun             RUN_FreeRun
        // Emulator -> UnloadProgram  PSTATE_1_NoProgramLoaded                   RUN_NotSpecified
        // Emulator -> SingleStep     PSTATE_3_ProgramRunningSingleStep          RUN_SingleStep
        // Emulator -> Stop           PSTATE_5_Aborted                           RUN_NotSpecified
        // PSTATE_1_NoProgramLoaded
        // PSTATE_2_ProgramLoaded
        // PSTATE_3_ProgramRunningFreeRun
        // PSTATE_3_ProgramRunningRunWithBreakPoints
        // PSTATE_3_ProgramRunningSingleStep
        // PSTATE_4_ProgramPausedBreakPoint
        // PSTATE_4_ProgramPausedSingleStep
        // PSTATE_4_ProgramPaused
        // PSTATE_4_ProgramHalted
        // PSTATE_5_Aborted

        // SystemReset and UnloadProgram both clear key states and reset program state to NoProgramLoaded
        // Keystrokes accepted:
        //     [ ] All Emulator menu keys:
        //         <ctrl>-R   SystemReset           Any time                          PSTATE_1_NoProgramLoaded
        //         F5         Run with breakpoints  Program loaded, running or not    PSTATE_3_ProgramRunningRunWithBreakPoints
        //         <ctrl>-F5  FreeRun               Program loaded, running or not    PSTATE_3_ProgramRunningFreeRun
        //         <shft>-F5  Stop                  Program loaded and running        PSTATE_5_Aborted
        //         F7         Stop                  Program loaded and running        PSTATE_5_Aborted
        //         F8         UnloadProgram         Program loaded, running or not    PSTATE_1_NoProgramLoaded
        //         F10        SingleStep            Program loaded, running or not    PSTATE_3_ProgramRunningSingleStep
        //     [x] Enter and Escape, only in FreeRun and RunWithBreakPoint conditions when in HPL
        //         [x] Enter leaves ProgramHalted state like pressing the Start button on the real System/3
        //         [x] Esc exits the RunProgram method
        //     [x] Keystrokes only set via routed commands (or PreviewKeyUp at the window level?)
        //     [-] Keystrokes' CanExecute methods call this class: IsKeyWanted () or just ignore keys
        // [x] All members volatile and private accessible only through access methods which use lock (m_objLock)
        // [x] Add method for getting state text
        // [x] Add plumbing for getting state changes to x_lblProgramState in the status bar via EventHandler
        //     How to manage CEmulatorState along with m_bProgramLoaded and m_bInEmulator? Put the bools inside the class?
        // [x] Replace m_bProgramLoaded with IsProgramLoaded (true for states 2, 3, and 4)
        // [x] Replace m_bInEmulator with IsInEmulator (true for states 3, and 4)
        // [x] When in single-step, reset HPL display with execution of next instruction
        // [x] When not in single-step, reset HPL display with the keystroke that exits the HPL code
        // [ ] Update register values in UI when breakpoints are triggered
        // [x] Gray out register values in UI when running
        // [x] if in Halted mode, F10 sets single-step mode as well as escapes from the halt like Enter

        // References to m_bIsInEmulator:
        // protected AddTagLabels ()
        //     public DisassembleCodeTrace ()
        //     public DisassembleCodeFromEmulator ()
        //     public DisassembleCodeImage ()
        // private FormatOneOperandInst ()
        //     private DisassembleInstruction ()
        // private AnnotateOneOperandInst ()
        // protected CalcAddress ()
        // protected LoadIndexRegister ()
        // private AnnotateCommandInst ()
        // private AccumulateData ()
        // private InsertHeaderLines ()
        //     private FormatTwoOperandInst ()
        //         private DisassembleInstruction ()
        // 	        private DisassembleCodeInternal ()
        // 		        public DisassembleCodeTrace ()
        // 		        public DisassembleCodeFromEmulator ()

        CEmulatorEngine m_objEmulatorEngine = null;
        private bool    m_bFirstTraceOutput = false;
        volatile ERunMode      m_eRunMode      = ERunMode.RUN_NotSpecified;
        volatile EProgramState m_eProgramState = EProgramState.PSTATE_Unspecified;

        public void SetFirstTraceOutput ()   { m_bFirstTraceOutput = true; }
        public void ResetFirstTraceOutput () { m_bFirstTraceOutput = false; }

        public CEmulatorState (CEmulatorEngine objEmulatorEngine)
        {
            m_objEmulatorEngine = objEmulatorEngine;
        }

        public EProgramState GetProgramState ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                return m_eProgramState;
            }
        }

        public void SetPaused ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                ChangeState (EProgramState.PSTATE_4_ProgramPaused);
            }
        }

        public void SetHalted ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                ChangeState (m_eRunMode == ERunMode.RUN_SingleStep ? EProgramState.PSTATE_4_ProgramPaused : EProgramState.PSTATE_4_ProgramHalted);
            }
        }

        public bool IsProgramLoaded ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                return m_eProgramState != EProgramState.PSTATE_1_NoProgramLoaded;
            }
        }

        public bool IsInEmulator ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                return m_eProgramState == EProgramState.PSTATE_2_ProgramLoaded ||
                       m_eProgramState.ToString ().Contains ("_3_")            ||
                       m_eProgramState.ToString ().Contains ("_4_");
            }
        }

        public bool IsRunning ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                return m_eProgramState.ToString ().Contains ("_3_");
            }
        }

        public bool IsFreeRun ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                return m_eRunMode == ERunMode.RUN_FreeRun;
            }
        }

        public bool IsRunWithBreakPoints ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                return m_eRunMode == ERunMode.RUN_BreakPoints;
            }
        }

        public bool IsSingleStep ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                return m_eRunMode == ERunMode.RUN_SingleStep;
            }
        }

        public bool IsPaused ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                return m_eProgramState.ToString ().Contains ("_4_");
            }
        }

        public bool IsProgramDead ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                return m_eProgramState.ToString ().Contains ("_5_");
            }
        }

        public bool IsHalted ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                return m_eProgramState == EProgramState.PSTATE_4_ProgramHalted;
            }
        }

        public bool IsAborted ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                return m_eProgramState == EProgramState.PSTATE_5_Aborted;
            }
        }

        public bool IsProgramUnloaded ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                return m_eProgramState == EProgramState.PSTATE_1_NoProgramLoaded;
            }
        }

        public string GetProgramStateString ()
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                if (m_eProgramState == EProgramState.PSTATE_1_NoProgramLoaded)
                {
                    return STATE_NOT_LOADED;
                }
                else if (m_eProgramState == EProgramState.PSTATE_2_ProgramLoaded)
                {
                    return STATE_LOADED;
                }
                else if (m_eProgramState == EProgramState.PSTATE_3_ProgramRunningFreeRun)
                {
                    return PSTATE_PROGRAM_RUNNING_FREE_RUN;
                }
                else if (m_eProgramState == EProgramState.PSTATE_3_ProgramRunningRunWithBreakPoints)
                {
                    return PSTATE_PROGRAM_RUNNING_RUN_WITH_BREAK_POINT;
                }
                else if (m_eProgramState == EProgramState.PSTATE_3_ProgramRunningSingleStep)
                {
                    return PSTATE_PROGRAM_RUNNING_SINGLE_STEP;
                }
                //else if (m_eProgramState == EProgramState.PSTATE_4_ProgramPausedBreakPoint)
                //{
                //    return PSTATE_PROGRAM_PAUSED_BREAKPOINT;
                //}
                //else if (m_eProgramState == EProgramState.PSTATE_4_ProgramPausedSingleStep)
                //{
                //    return PSTATE_PROGRAM_PAUSED_SINGLESTEP;
                //}
                else if (m_eProgramState == EProgramState.PSTATE_4_ProgramPaused)
                {
                    return PSTATE_PROGRAM_PAUSED;
                }
                else if (m_eProgramState == EProgramState.PSTATE_4_ProgramHalted)
                {
                    return PSTATE_PROGRAM_HALTED;
                }
                else if (m_eProgramState == EProgramState.PSTATE_5_Aborted)
                {
                    return PSTATE_PROGRAM_ABORTED;
                }
                else if (m_eProgramState == EProgramState.PSTATE_5_PChk_InvalidOpCode)
                {
                    return PSTATE_PCHK_INVALID_OP_CODE;
                }
                else if (m_eProgramState == EProgramState.PSTATE_5_PChk_InvalidQByte)
                {
                    return PSTATE_PCHK_INVALID_Q_BYTE;
                }
                else if (m_eProgramState == EProgramState.PSTATE_5_PChk_AddressWrap)
                {
                    return PSTATE_PCHK_ADDRESS_WRAP;
                }
                else if (m_eProgramState == EProgramState.PSTATE_5_PChk_InvalidAddress)
                {
                    return PSTATE_PCHK_INVALID_ADDRESS;
                }
                else if (m_eProgramState == EProgramState.PSTATE_5_PChk_PL2_Unsupported)
                {
                    return PSTATE_PCHK_PL2_UNSUPPORTED;
                }
                else if (m_eProgramState == EProgramState.PSTATE_5_PChk_UnsupportedDevice)
                {
                    return PSTATE_PCHK_UNSUPPORTED_DEVICE;
                }
                else
                {
                    return "";
                }
            }
        }

        public void SetRunState (ERunMode eRunMode = ERunMode.RUN_NotSpecified)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                if (m_eRunMode == eRunMode)
                {
                    return;
                }

                if (eRunMode == ERunMode.RUN_NotSpecified)
                {
                    eRunMode = m_eRunMode;
                }
                else
                {
                    m_eRunMode = eRunMode;
                }

                Console.WriteLine ("SetRunState ()  " + m_eProgramState.ToString () + "  " + m_eRunMode.ToString ());
                if (m_eProgramState != EProgramState.PSTATE_1_NoProgramLoaded)
                {
                    if (m_eRunMode == ERunMode.RUN_FreeRun)
                    {
                        ChangeState (EProgramState.PSTATE_3_ProgramRunningFreeRun);
                        m_objEmulatorEngine.FireOnNewEnableHighlightLineEvent (false);
                        m_objEmulatorEngine.FireClearGrayedLinesListEvent ();
                        if (m_objEmulatorEngine.IsInTrace ())
                        {
                            Console.WriteLine ("  SetRunState ()  Stop Trace");
                            m_objEmulatorEngine.ResetTrace ();
                            m_objEmulatorEngine.ResetShowMFCUBuffers ();
                            m_objEmulatorEngine.ResetShowPrinterBuffer ();
                            m_objEmulatorEngine.ResetShowDiskBuffers ();
                            //m_objEmulatorEngine.WriteOutput ("- - - - - - - - - - - - - - - - - - - - - - - - < S T O P    T R A C E > - - - - - - - - - - - - - - - - - - - - - - - -",
                            m_objEmulatorEngine.WriteOutput ("                                                < S T O P    T R A C E >",
                                                             CEmulatorEngine.EOutputTarget.OUTPUT_TracePanel);
                        }
                        //m_objEmulatorEngine.FireMakeRegisterLabelsDormantEvent ();
                    }
                    else if (m_eRunMode == ERunMode.RUN_BreakPoints)
                    {
                        ChangeState (EProgramState.PSTATE_3_ProgramRunningRunWithBreakPoints);
                        m_objEmulatorEngine.FireOnNewEnableHighlightLineEvent (false);
                        m_objEmulatorEngine.FireClearGrayedLinesListEvent ();
                        //m_objEmulatorEngine.FireMakeRegisterLabelsDormantEvent ();
                        if (m_objEmulatorEngine.IsInTrace ())
                        {
                            m_objEmulatorEngine.ResetTrace ();
                            m_objEmulatorEngine.SetShowMFCUBuffers ();
                            m_objEmulatorEngine.ResetShowPrinterBuffer ();
                            m_objEmulatorEngine.ResetShowDiskBuffers ();
                        }
                    }
                    else
                    {
                        ChangeState (EProgramState.PSTATE_3_ProgramRunningSingleStep);
                        m_objEmulatorEngine.FireOnNewEnableHighlightLineEvent (true);
                        //m_objEmulatorEngine.FireMakeRegisterLabelsActiveEvent ();
                        if (!m_objEmulatorEngine.IsInTrace () &&
                            !m_objEmulatorEngine.GetManuallyReset ())
                        {
                            Console.WriteLine ("  SetRunState ()  Start Trace");
                            m_objEmulatorEngine.SetTrace ();
                            m_objEmulatorEngine.SetShowDisassembly ();
                            m_objEmulatorEngine.SetShowChangedValues ();
                            m_objEmulatorEngine.SetShowIOBuffers ();
                            //m_objEmulatorEngine.WriteOutput ("- - - - - - - - - - - - - - - - - - - - - - - - < S T A R T   T R A C E > - - - - - - - - - - - - - - - - - - - - - - - -",
                            if (m_bFirstTraceOutput)
                            {
                                m_objEmulatorEngine.WriteOutput ("                                                < S T A R T   T R A C E >",
                                                                 CEmulatorEngine.EOutputTarget.OUTPUT_TracePanel);
                            }
                            m_bFirstTraceOutput = true;
                        }
                    }

                    m_objEmulatorEngine.FireShowTraceStateEvent ();
                }
            }
        }

        // Create a class to manage keystrokes and program states: CEmulatorState
        //   Implement state machine:
        //     State 1 NoProgramLoaded con only change to state 2 ProgramLoaded
        //     State 2 ProgramLoaded can only change to one of the state 3 ProgramRunning conditions
        //     When in state 3 or 4, all conditions in states 3, 4, and 5 are available
        //     State 5 is dead-end game-over and can only change to State 1 NoProgramLoaded
        public void ChangeState (EProgramState eProgramState)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("ChangeState (" + eProgramState.ToString () + ") old: " + m_eProgramState.ToString ());
                if (m_eProgramState == eProgramState)
                {
                    //Console.WriteLine ("  ChangeState: new and old values match");
                    return;
                }

                if (m_eProgramState == EProgramState.PSTATE_1_NoProgramLoaded &&
                    eProgramState   != EProgramState.PSTATE_2_ProgramLoaded)
                {
                    throw new Exception (CEmulatorEngine.INVALID_EPROGRAMSTATE_CHANGE);
                }
                else if (m_eProgramState == EProgramState.PSTATE_2_ProgramLoaded &&
                         !eProgramState.ToString ().Contains ("_3_"))
                {
                    throw new Exception (CEmulatorEngine.INVALID_EPROGRAMSTATE_CHANGE);
                }
                //else if (m_eProgramState.ToString ().Contains ("_5_") &&
                //         eProgramState != EProgramState.PSTATE_1_NoProgramLoaded)
                //{
                //    throw new Exception (CEmulatorEngine.INVALID_EPROGRAMSTATE_CHANGE);
                //}
                else if (eProgramState == EProgramState.PSTATE_4_ProgramHalted &&
                         m_eRunMode == ERunMode.RUN_SingleStep)
                {
                    throw new Exception (CEmulatorEngine.INVALID_EPROGRAMSTATE_CHANGE);
                }
                //else if (m_eProgramState.ToString ().Contains ("_4_") &&
                //         eProgramState.ToString ().Contains   ("_5_"))
                //{
                //    throw new Exception (CEmulatorEngine.INVALID_EPROGRAMSTATE_CHANGE);
                //}
                else if (m_eProgramState.ToString ().Contains ("_3_") &&
                         !eProgramState.ToString ().Contains  ("_3_") &&
                         !eProgramState.ToString ().Contains  ("_4_") &&
                         !eProgramState.ToString ().Contains  ("_5_"))
                {
                    throw new Exception (CEmulatorEngine.INVALID_EPROGRAMSTATE_CHANGE);
                }

                m_eProgramState = eProgramState;
                m_objEmulatorEngine.FireProgramStateEvent (GetProgramStateString ());
                if (m_eProgramState == EProgramState.PSTATE_3_ProgramRunningFreeRun ||
                    m_eProgramState == EProgramState.PSTATE_3_ProgramRunningRunWithBreakPoints)
                {
                    m_objEmulatorEngine.FireMakeRegisterLabelsDormantEvent ();
                    m_objEmulatorEngine.FireNewCPUDialsEvent ((short)m_objEmulatorEngine.GetConsoleDials (), false);
                    m_objEmulatorEngine.FireOnNewEnableHighlightLineEvent (false);
                }
                else if (m_eProgramState == EProgramState.PSTATE_3_ProgramRunningSingleStep)
                {
                    m_objEmulatorEngine.FireMakeRegisterLabelsActiveEvent ();
                    m_objEmulatorEngine.FireNewCPUDialsEvent ((short)m_objEmulatorEngine.GetConsoleDials (), true);
                    m_objEmulatorEngine.FireOnNewEnableHighlightLineEvent (true);
                }
                else if (m_eProgramState == EProgramState.PSTATE_2_ProgramLoaded ||
                         m_eProgramState == EProgramState.PSTATE_1_NoProgramLoaded)
                {
                    m_objEmulatorEngine.FireUpdateAppTitleEvent ();
                }

                if (eProgramState.ToString ().Contains ("_5_"))
                {
                    if (eProgramState == EProgramState.PSTATE_5_PChk_InvalidOpCode)
                    {
                        m_objEmulatorEngine.FireProcessorCheckEvents (PSTATE_PROCESSOR_CHECK, PSTATE_PCHK_INVALID_OP_CODE_2);
                    }
                    else if (eProgramState == EProgramState.PSTATE_5_PChk_InvalidQByte)
                    {
                        m_objEmulatorEngine.FireProcessorCheckEvents (PSTATE_PROCESSOR_CHECK, PSTATE_PCHK_INVALID_Q_BYTE_2);
                    }
                    else if (eProgramState == EProgramState.PSTATE_5_PChk_AddressWrap)
                    {
                        m_objEmulatorEngine.FireProcessorCheckEvents (PSTATE_PROCESSOR_CHECK, PSTATE_PCHK_ADDRESS_WRAP_2);
                    }
                    else if (eProgramState == EProgramState.PSTATE_5_PChk_InvalidAddress)
                    {
                        m_objEmulatorEngine.FireProcessorCheckEvents (PSTATE_PROCESSOR_CHECK, PSTATE_PCHK_INVALID_ADDRESS_2);
                    }
                    else if (eProgramState == EProgramState.PSTATE_5_PChk_PL2_Unsupported)
                    {
                        m_objEmulatorEngine.FireProcessorCheckEvents (PSTATE_PROCESSOR_CHECK, PSTATE_PCHK_PL2_UNSUPPORTED_2);
                    }
                    else if (eProgramState == EProgramState.PSTATE_5_PChk_UnsupportedDevice)
                    {
                        m_objEmulatorEngine.FireProcessorCheckEvents (PSTATE_PROCESSOR_CHECK, PSTATE_PCHK_UNSUPPORTED_DEVICE_2);
                    }
                }
                else
                {
                    m_objEmulatorEngine.FireProcessorCheckEvents ("", "");
                }
            }
        }
    }

    public class CPrintQueue
    {
        private int m_iBlankLineCount = 0;
        private List<string> m_lstrQueuedLines       = new List<string> ();
        private List<string> m_lstrPrinterPanelLines = new List<string> ();
        CEmulatorEngine m_objEmulatorEngine = null;

        public CPrintQueue (CEmulatorEngine objEmulatorEngine)
        {
            m_objEmulatorEngine = objEmulatorEngine;
        }

        public void Enqueue (List<string> lstrPrintLines, bool bSuppressTrailingBlankLines = true, bool bPrintNow = false)
        {
            foreach (string str in lstrPrintLines)
            {
                Enqueue (str, bSuppressTrailingBlankLines);
            }

            if (bPrintNow)
            {
                PrintQueuedLines ();
            }
        }

        public void Enqueue (string strPrintLine, bool bSuppressTrailingBlankLines = true, bool bPrintNow = false)
        {
            if (CEmulatorEngine.IsAllBlank (strPrintLine) &&
                bSuppressTrailingBlankLines)
            {
                ++m_iBlankLineCount;
            }
            else
            {
                while (m_iBlankLineCount > 0)
                {
                    m_lstrQueuedLines.Add ("");
                    --m_iBlankLineCount;
                }

                m_lstrQueuedLines.Add (strPrintLine);
            }

            if (bPrintNow)
            {
                PrintQueuedLines ();
            }
        }

        public void PrintQueuedLines ()
        {
            if (m_lstrQueuedLines.Count == 0)
            {
                return;
            }

            m_lstrPrinterPanelLines.AddRange (m_lstrQueuedLines);
            m_objEmulatorEngine.FireNewPrinterStringListEvent (m_lstrQueuedLines);

            m_iBlankLineCount = 0;
            m_lstrQueuedLines.Clear ();
        }

        public bool HasQueuedLines ()
        {
            return m_lstrPrinterPanelLines.Count > 0;
        }

        public void WriteToFile (string strFilename)
        {
            if (m_lstrPrinterPanelLines.Count > 0 &&
                strFilename.Length > 0)
            {
                File.WriteAllLines (strFilename, m_lstrPrinterPanelLines.ToArray ());
            }
        }

        public void Clear ()
        {
            m_iBlankLineCount = 0;
            m_lstrQueuedLines.Clear ();
            m_lstrPrinterPanelLines.Clear ();
        }
    }

    public class CTraceQueue
    {
        protected bool m_bInTrace           = false;
        protected bool m_bShowMFCUBuffers   = false;
        protected bool m_bShowPrinterBuffer = false;
        protected bool m_bShowDiskBuffers   = false;

        private List<string> m_lstrQueuedLines     = new List<string> ();
        private List<string> m_lstrTracePanelLines = new List<string> ();
        CEmulatorEngine m_objEmulatorEngine = null;

        public CTraceQueue (CEmulatorEngine objEmulatorEngine)
        {
            m_objEmulatorEngine = objEmulatorEngine;
        }

        public void EnqueueTraceLine (string strTraceLine)
        {
            if (m_bInTrace)
            {
                m_lstrQueuedLines.Add (strTraceLine);
            }
            //Console.WriteLine ("EnqueueTraceLine (" + strTraceLine + ')');
            m_lstrTracePanelLines.Add (strTraceLine);
        }

        public void EnqueueTraceLines (List<string> lstrTraceLines)
        {
            if (m_bInTrace)
            {
                m_lstrQueuedLines.AddRange (lstrTraceLines);
            }
            //Console.WriteLine ("EnqueueTraceLine (" + lstrTraceLines.Count.ToString () + ')');
            m_lstrTracePanelLines.AddRange (lstrTraceLines);
        }

        public void EnqueueMFCULine (string strMFCULine)
        {
            if (//m_bInTrace &&
                m_bShowMFCUBuffers)
            {
                m_lstrQueuedLines.Add (strMFCULine);
            }
            m_lstrTracePanelLines.Add (strMFCULine);
        }

        public void EnqueueMFCULines (List<string> lstrMFCULines)
        {
            if (//m_bInTrace &&
                m_bShowMFCUBuffers)
            {
                m_lstrQueuedLines.AddRange (lstrMFCULines);
            }
            m_lstrTracePanelLines.AddRange (lstrMFCULines);
        }

        public void EnqueuePrinterLine (string strPrinterLine)
        {
            if (//m_bInTrace &&
                m_bShowPrinterBuffer)
            {
                m_lstrQueuedLines.Add (strPrinterLine);
            }
            m_lstrTracePanelLines.Add (strPrinterLine);
        }

        public void EnqueuePrinterLines (List<string> lstrPrinterLines)
        {
            if (//m_bInTrace &&
                m_bShowPrinterBuffer)
            {
                m_lstrQueuedLines.AddRange (lstrPrinterLines);
            }
            m_lstrTracePanelLines.AddRange (lstrPrinterLines);
        }

        public void EnqueueDiskLine (string strDiskLine)
        {
            if (//m_bInTrace &&
                m_bShowDiskBuffers)
            {
                m_lstrQueuedLines.Add (strDiskLine);
            }
            m_lstrTracePanelLines.Add (strDiskLine);
        }

        public void EnqueueDiskLines (List<string> lstrDiskLines)
        {
            if (//m_bInTrace &&
                m_bShowDiskBuffers)
            {
                m_lstrQueuedLines.AddRange (lstrDiskLines);
            }
            m_lstrTracePanelLines.AddRange (lstrDiskLines);
        }

        public void UpdateAndOutputToTracePanel ()
        {
            if (m_bInTrace)
            {
                m_objEmulatorEngine.FireNewTraceStringListEvent (m_lstrQueuedLines);
                m_lstrQueuedLines.Clear ();
            }

            UpdateOnly ();
        }

        public void UpdateOnly ()
        {
            m_bInTrace           = m_objEmulatorEngine.IsInTrace ();
            m_bShowMFCUBuffers   = m_objEmulatorEngine.GetShowMFCUBuffers ();
            m_bShowPrinterBuffer = m_objEmulatorEngine.GetShowPrinterBuffer ();
            m_bShowDiskBuffers   = m_objEmulatorEngine.GetShowDiskBuffers ();
        }

        public bool HasQueuedLines ()
        {
            return m_lstrTracePanelLines.Count > 0;
        }

        public void WriteToFile (string strFilename)
        {
            if (m_lstrTracePanelLines.Count > 0 &&
                strFilename.Length > 0)
            {
                File.WriteAllLines (strFilename, m_lstrTracePanelLines.ToArray ());
            }
        }

        public void AppendToFile (string strFilename)
        {
            if (m_lstrTracePanelLines.Count > 0 &&
                strFilename.Length > 0)
            {
                File.AppendAllLines (strFilename, m_lstrTracePanelLines.ToArray ());
            }
        }

        public void Clear ()
        {
            m_lstrQueuedLines.Clear ();
            m_lstrTracePanelLines.Clear ();
        }
    }

    public class CEmulatorEngine : CDataConversion
    {
        // IO registers depending on device
        // RAM used by & affected by instruction
        // RAM depending on device & operation

        #region System/3 registers
        protected int[] m_iaIAR = { 0, 0, 0, 0, 0, 0 } ;  // Instruction Address Register array
        protected int m_iIARpl1 = 0;  // Instruction Address Register (DPF Program Level 1)
        protected int m_iIARpl2 = 0;  // Instruction Address Register (DPF Program Level 2)
        protected int[] m_iaARR = { 0, 0, 0, 0, 0, 0 } ;  // Address Recall Register array
        protected int m_iARRpl1 = 0;  // Address Recall Register (DPF Program Level 1)
        protected int m_iARRpl2 = 0;  // Address Recall Register (DPF Program Level 2)
        protected int m_iXR1    = 0;  // Index Register 1
        protected int m_iXR2    = 0;  // Index Register 2
        protected int m_iIL     = 0;  // Index into m_iaIAR / m_iaARR / m_aCR arrays by interrupt level + 1 (not a real register in the System/3)
        protected int m_iHPL_IL = -1; // Interrupt Level of last HPL
        protected CConditionRegister[] m_aCR = { new CConditionRegister (),   // [0] Main program, no interrupt
                                                 new CConditionRegister (),   // [1] Interrupt Level 0 - Dual Programming Control Interrupt Key
                                                 new CConditionRegister (),   // [2] Interrupt Level 1 - 5475 or 5471 keyboard
                                                 new CConditionRegister (),   // [3] Interrupt Level 2 - BSCA (Binary Synchronous Communications Adapter)
                                                 new CConditionRegister (),   // [4] Interrupt Level 3 - Unassigned
                                                 new CConditionRegister () }; // [5] Interrupt Level 4 - Serial I/O Channel
        const int IL_Main                 = 0;
        const int IL_0_DPF_Interrupt      = 1;
        const int IL_1_Keyboard           = 2;
        const int IL_2_BSCA               = 3;
        const int IL_3_Unassigned         = 4;
        const int IL_4_Serial_IO          = 5;
        const int BUSY_COUNT_WAIT         = 10;
        const int SYSTEM_3_CPU_TIME_DELAY = 203; // Instruction count before 2ms sleep (202: 1005ms, 203: 1000ms, 204: 995ms, 205: 990ms, 206: 985ms debug)

        // System/3 main memory
        //protected byte[] m_yaMainMemory = new byte[64 * 1024];

        // System/3 reserved memory areas
        protected int m_iMemorySizeInK;
        protected string m_strSystemDate;
        //protected string m_strPrinterChainImage;
        protected string m_strCopyright;

        protected bool m_bMemorySizeSet        = false;
        protected bool m_bSystemDateSet        = false;
        protected bool m_bPrinterChainImageSet = false;
        protected bool m_bCopyrightSet         = false;
#endregion

        #region Emulator member data
        //private const string DATABASE_PATH = @"D:\SoftwareDev\SacredCat\IBMSystem3\Databases\Symulator3.accdb";
        private const string DATABASE_PATH_DEBUG          = @"..\..\..\..\..\Databases\Symulator3.accdb";
        private const string DATABASE_PATH_RELEASE        = "Symulator3.accdb";
        private const string ESCAPE_KEY                   = "Escape Key";
        public  const string INVALID_EPROGRAMSTATE_CHANGE = "Invalid EProgramState change";
        public  const int    CARD_IMAGE_SIZE              = 0x005F;
        public  const int    CARD_IMAGE_IPL_SIZE          = 0x003F;
        protected byte[] ABSOLUTE_CARD_LOADER_START = { 0xC2, 0x01, 0x00, 0x00, 0x5C, 0x0B, 0xFF, 0x2B, 0x5C, 0x07, 0x67, 0x33, 0x3A, 0x0F, 0x01, 0x7E, 0xD0, 0x00, 0xF4 };

        private EBootDevice          m_eBootDevice           = EBootDevice.BOOT_Card;
        private EOutputTarget        m_eOutputTarget         = EOutputTarget.OUTPUT_Console;
        private EUIRunMode           m_eUIRunMode            = EUIRunMode.RUN_Undefined;
        private bool                 m_bAsyncRun             = false;
        private bool                 m_bMFCUBufferRulerShown = false;
        public  CEmulatorState       m_objEmulatorState      = null;
        public  CPrintQueue          m_objPrintQueue         = null;
        public  CTraceQueue          m_objTraceQueue         = null;
        public  object               m_objLock               = new Object ();

        public void SetBootDevice (EBootDevice eBootDevice)
        {
            m_eBootDevice = eBootDevice;
            //m_i5203LineWidth = m_eBootDevice == EBootDevice.BOOT_Card ? 96 : 132;
            if (m_eBootDevice == EBootDevice.BOOT_Card)
            {
                m_obj5203LinePrinter.SetPrinterWidth96 ();
            }
            else
            {
                m_obj5203LinePrinter.SetPrinterWidth132 ();
            }
        }
        //private string m_strDatabasePath = "";

        public void RotateCPUClock ()
        {
            if (m_bSimulateSystem3CpuTiming)
            {
                SetTestOneCardClockProgram ();
            }
            else if (m_bTestOneCardClockProgram)
            {
                ResetTestOneCardClockProgram ();
            }
            else
            {
                SetSimulateSystem3CpuTiming ();
            }
        }

        public void SetOutputTarget (EOutputTarget eOutputTarget)
        {
            m_eOutputTarget = eOutputTarget;
        }

        byte[] m_ya48CharacterPrinterChainImage =
            { 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xF0, 0x7B, 0x7C, 0x61, 0xE2, 0xE3, 0xE4,
              0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0x50, 0x6B, 0x6C, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8,
              0xD9, 0x60, 0x5B, 0x5C, 0xC1, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0x4E, 0x4B, 0x7D };

        protected ushort m_usConsoleDialSetting   = 0x0000;
        protected int    m_iInstructionCount      = 0;
        protected int    m_iInstructionCountLimit = 100000;
        protected int    m_iHaltCount             = 0;
        protected int    m_iHaltCountLimit        = 1000;
        protected int    m_iSleepInterval         = 10; // In milliseconds for Sleep ()
        protected int    m_iTraceTriggerIAR       = 0x00010000;
        protected int    m_iInstructionDelayCount = -1;
        protected int    m_iOldStartDASMAddress   = 0x0000;
        protected int    m_iOldEndDASMAddress     = CARD_IMAGE_SIZE;
        protected int    m_iOldDASMEntryPoint     = 0x0000;
        protected int    m_iNewStartDASMAddress   = 0x0000;
        protected int    m_iNewEndDASMAddress     = 0x0000;
        protected int    m_iNewDASMEntryPoint     = 0x0000;
        protected int    m_iIPLCardCount          = 0;
        protected int    m_iIPLCardReadCount      = 0;
        protected int    m_iProgramSizeIPL        = 0;
        protected bool   m_bIsAbsoluteCardLoader  = false;
        protected bool   m_bRunAfterDASM          = true;
        protected bool   m_bLoadingComplete       = false;
        protected bool   m_bFirstTime             = true; // DEBUG_DISK_IPL
        protected bool   m_bKeyboardInputEnabled  = false;
        protected bool   m_bDataKeysEnabled       = false;
        protected char   m_cKeyStroke             = '\x00';
        protected bool   m_bKeyCtrl               = false;
        protected bool   m_bKeyAlt                = false;
        protected string m_strKeyStroke           = "";

        //public void SetDatabasePath          (string strDatabasePath)     { m_strDatabasePath        = strDatabasePath; }
        public void SetConsoleDials          (ushort usConsoleDialSetting)    { m_usConsoleDialSetting    = (ushort)(usConsoleDialSetting & 0xFFFF); }
        public void SetInstructionCountLimit (int iInstructionCountLimit) { m_iInstructionCountLimit = iInstructionCountLimit; }
        public void SetHaltCountLimit        (int iHaltCountLimit)        { m_iHaltCountLimit        = iHaltCountLimit; }
        public void SetSleepInterval         (int iSleepInterval)         { m_iSleepInterval         = iSleepInterval; }
        public void SetDasmTriggerAddress    (int iDasmTriggerAddress)    { m_iDasmTriggerAddress    = iDasmTriggerAddress; }
        public void SetDasmStartAddress      (int iDasmStartAddress)      { m_iDasmStartAddress      = iDasmStartAddress; }
        public void SetDasmEndAddress        (int iDasmEndAddress)        { m_iDasmEndAddress        = iDasmEndAddress; }
        public ushort GetConsoleDials        ()                           { return m_usConsoleDialSetting; }
        public bool IsKeyboardInputEnabled   ()                           { return m_bKeyboardInputEnabled; }
        public bool IsDataKeyInputEnabled    ()                           { return m_bDataKeysEnabled; }
        public void SetKeyStroke             (char cKeyStroke)            { m_cKeyStroke   = cKeyStroke; }
        public void SetKeyStroke             (string strKeyStroke)        { m_strKeyStroke = strKeyStroke; }
        public void SetKeyCtrl               (bool bKeyCtrl)              { m_bKeyCtrl = bKeyCtrl; }
        public void SetKeyAlt                (bool bKeyAlt)               { m_bKeyAlt  = bKeyAlt; }
        public void ResetTriggeredDasm ()
        {
             m_iDasmTriggerAddress = 0x00010000;
             m_iDasmStartAddress   = 0x0000;
             m_iDasmEndAddress     = 0x0000;
        }

        public int  GetProgramSizeIPL () { return m_iProgramSizeIPL; }

        public void SetIPLCardCount (int iIPLCardCount) { m_iIPLCardCount = iIPLCardCount; }

        public bool IsAbsoluteCardLoader ()
        {
            int iBinaryImageIdx     = 0x0000;
            m_bIsAbsoluteCardLoader = true;

            foreach (byte y in ABSOLUTE_CARD_LOADER_START)
            {
                if (m_yaMainMemory[iBinaryImageIdx++] != y)
                {
                    m_bIsAbsoluteCardLoader = false;
                }
            }

            if (m_bIsAbsoluteCardLoader)
            {
                ResetShowMFCUBuffers ();
            }

            return m_bIsAbsoluteCardLoader;
        }

        public byte[] ReadCardIPLfromDB ()
        {
            return m_obj5424MFCU.ReadCardFromPrimaryIPL ();
        }

        protected bool m_bClearMemory = true;
        public void SetClearMemory ()   { m_bClearMemory = true; }
        public void ResetClearMemory () { m_bClearMemory = true; }
        public bool GetClearMemory ()   { return m_bClearMemory; }
        #endregion

        #region I/O device data - 5471 Printer/Keyboard Console
        private StringBuilder m_strb5471PrinterOutput = new StringBuilder ();
        private StringBuilder m_strb5471KeyboardInput = new StringBuilder ();
        private List<string>  m_strl5471Output        = new List<string> ();
        private byte          m_y5471PrinterOutput    = (byte)' ';
        private byte          m_y5471KeyboardInput    = (byte)' ';
        private int           m_i5471PrinterFormWidth = 80;
        private int           m_i5471StatusFlags      = 0;
        private int           m_iResetBusyCount       = 0; // For triggering 5471 printer interrupt

        // Internal 5471 Status Bit Values
        const int STAT_Prt5471_End_Of_Form                                = 0x00010000;
        const int STAT_Prt5471_End_Of_Line                                = 0x00008000;
        const int STAT_Prt5471_Printer_Busy                               = 0x00004000;
        const int STAT_Prt5471_Non_Printable_Character                    = 0x00002000;
        const int STAT_Prt5471_Printer_Interrupt_Pending                  = 0x00001000;
        const int STAT_Prt5471_Printer_Interrupt_Enabled                  = 0x00000800;
        const int STAT_Kbd5471_Request_Key_Interrupt                      = 0x00000400;
        const int STAT_Kbd5471_Key_Interrupt_Pending                      = 0x00000200;
        const int STAT_Kbd5471_Return_Key                                 = 0x00000100;
        const int STAT_Kbd5471_Return_Or_Data_Key_Interrupt_Pending       = 0x00000080;
        const int STAT_Kbd5471_End_Key                                    = 0x00000040;
        const int STAT_Kbd5471_Cancel_Key                                 = 0x00000020;
        const int STAT_Kbd5471_End_Or_Cancel_Interrupt_Pending            = 0x00000010;
        const int STAT_Kbd5471_Proceed_Pending_Indicator_On               = 0x00000008;
        const int STAT_Kbd5471_Request_Pending_Indicator_On               = 0x00000004;
        const int STAT_Kbd5471_Other_Interrupts_Enabled                   = 0x00000002;
        const int STAT_Kbd5471_Request_Key_Interrupt_Enabled              = 0x00000001;

        // System/3-Defined SNS Status Bits
        const int k5471_SNS_KBD_Request_Key_Interrupt_Pending             = 0x0080;
        const int k5471_SNS_KBD_End_or_Cancel_Interrupt_Pending           = 0x0040;
        const int k5471_SNS_KBD_Cancel_Key                                = 0x0020;
        const int k5471_SNS_KBD_End_Key                                   = 0x0010;
        const int k5471_SNS_KBD_Return_or_Data_Key_Interrupt_Pending      = 0x0008;
        const int k5471_SNS_KBD_Return_Key                                = 0x0004;
        const int k5471_SNS_KBD_Request_Key_Interrupt_Enabled             = 0x0080;
        const int k5471_SNS_KBD_Other_Key_Interrupt_Enabled               = 0x0040;
        const int k5471_SNS_PRT_Printer_Interrupt_Pending                 = 0x0080;
        const int k5471_SNS_PRT_Non_Printable_Character                   = 0x0020;
        const int k5471_SNS_PRT_Printer_Busy                              = 0x0010;
        const int k5471_SNS_PRT_End_of_Line                               = 0x0008;

        // System/3-Defined SIO Control Code Bits
        const int k5471_SIO_KBD_Request_Pending_Indicator_On_or_Off       = 0x20;
        const int k5471_SIO_KBD_Proceed_Indicator_On_or_Off               = 0x10;
        const int k5471_SIO_KBD_Request_Key_Interrupts_Enable_Disable     = 0x04;
        const int k5471_SIO_KBD_Other_Key_Interrupts_Enable_Disable       = 0x02;
        const int k5471_SIO_KBD_Reset_Request_Key_or_Other_Key_Interrupts = 0x01;
        const int k5471_SIO_PRT_Start_Print                               = 0x80;
        const int k5471_SIO_PRT_Start_Carrier_Return                      = 0x40;
        const int k5471_SIO_PRT_Printer_Interrupt_Enable_Disable          = 0x04;
        const int k5471_SIO_PRT_Reset_Printer_Interrupt                   = 0x01;

        ushort[] m_usaiTestSnsValues = { 0x6108,   // '/'
                                         0x6108,   // '/'
                                         0x4008,   // ' '
                                         0xC408,   // 'D'
                                         0xC108,   // 'A'
                                         0xE308,   // 'T'
                                         0xC508,   // 'E'
                                         0x4008,   // ' '
                                         0xF108,   // '1'
                                         0xF208,   // '2'
                                         0xF308,   // '3'
                                         0xF408,   // '4'
                                         0xF508,   // '5'
                                         0xF608,   // '6'
                                         0x0D50 }; // '<cr>'
        int m_iTestSnsValueIdx = 0;
        #endregion

        #region I/O device data - 5475 Data Entry Keyboard
        private enum E5475Interrupt
        {
            E5475_Interrupt_None,
            E5475_Interrupt_Data_Key,
            E5475_Interrupt_Function_Key,
            E5475_Interrupt_Multi_Punch
        };

        private byte m_y5475KeyboardInput       = 0x00, // EBCDIC blank character
                     m_y5475Keystroke           = 0x00, // ASCII blank character
                     m_yMultiPunchCharacter     = 0x00,
                     m_y5475KeyboardSNS001Low   = 0x00, // SNS 001 High contains keystroke character
                     m_y5475KeyboardSNS010High  = 0x00,
                     m_y5475KeyboardSNS010Low   = 0x40,
                     m_y5475KeyboardSNS011Low   = 0x4A; // SNS 011 High always 0x00
        private bool m_bMultiPunch              = false;
        private E5475Interrupt m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_None;
        private byte[] m_yaEbcdicPunchTable     = { 0x40, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7,
                                                    0xF8, 0xF9, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F,
                                                    0xF0, 0x61, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7,
                                                    0xE8, 0xE9, 0x50, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F,
                                                    0x60, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7,
                                                    0xD8, 0xD9, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F,
                                                    0xD0, 0xC1, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 
                                                    0xC8, 0xC9, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F };

        protected void Reset5475Sns001 ()
        {
            //m_y5475KeyboardInput     = 0x00;
            m_y5475KeyboardSNS001Low = 0x4A; // Turn off all but the "reserved" bits
        }

        protected void Reset5475Sns010 ()
        {
            m_y5475KeyboardSNS010High = 0x00;
            m_y5475KeyboardSNS010Low  = 0x20; // Turn off all but the "reserved" bit
        }

        protected void Reset5475Sns011 ()
        {
            m_y5475KeyboardSNS011Low = 0x00;
        }

        protected void Reset5475Keys ()
        {
            m_b5475_SNS_2H_Program_1_Key_Pressed         = false;           
            m_b5475_SNS_2H_Program_2_Key_Pressed         = false;           
            m_b5475_SNS_2H_Release_Key_Pressed           = false;           
            m_b5475_SNS_2H_Field_Erase_Key_Pressed       = false;           
            m_b5475_SNS_2H_Error_Reset_Key_Pressed       = false;           
            m_b5475_SNS_2H_Read_Key_Pressed              = false;           

            m_b5475_SNS_2L_Record_Erase_Switch_Operated  = false;
            m_b5475_SNS_2L_Skip_Key_Pressed              = false;
            m_b5475_SNS_2L_Dup_Key_Pressed               = false;

            m_b5475_SNS_3L_Any_Function_Key_Pressed      = false;
            m_b5475_SNS_3L_Any_Data_Key                  = false;
        }

        protected void Reset5475StatusFlags ()
        {
            if (!m_bEnableShow5475Status &&
                m_b5475_SNS_3L_Keyboard_Interrupts_Enabled)
            {
                m_b5475_Keys_Unlocked                    = false;
            }

            m_b5475_Data_Key_Latched                     = false;
            m_b5475_Program_One_Selected                 = false;
            m_b5475_Keyboard_Program_Numeric_Mode        = false;
            m_b5475_Keyboard_Program_Lower_Shift         = false;
            m_b5475_Error_Indicator_On                   = false;
            m_b5475_LIO_Program_1_Indicator              = false;
            m_b5475_LIO_Program_2_Indicator              = false;

            m_b5475_SNS_1L_Lower_Shift_Key_Pressed       = false;
            m_b5475_SNS_1L_Invalid_Character_Detected    = false;

            m_b5475_SNS_2H_Program_1_Key_Pressed         = false;           
            m_b5475_SNS_2H_Program_2_Key_Pressed         = false;           
            m_b5475_SNS_2H_Release_Key_Pressed           = false;           
            m_b5475_SNS_2H_Field_Erase_Key_Pressed       = false;           
            m_b5475_SNS_2H_Error_Reset_Key_Pressed       = false;           
            m_b5475_SNS_2H_Read_Key_Pressed              = false;           

            m_b5475_SNS_2L_Record_Erase_Switch_Operated  = false;
            m_b5475_SNS_2L_Skip_Key_Pressed              = false;
            m_b5475_SNS_2L_Dup_Key_Pressed               = false;

            m_b5475_SNS_3L_Any_Function_Key_Pressed      = false;
            m_b5475_SNS_3L_Any_Data_Key                  = false;

            m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_None;
        }

        protected bool m_b5475_Data_Key_Latched              = false;
        protected bool m_b5475_Keys_Unlocked                 = false;
        protected bool m_b5475_Program_One_Selected          = false;
        protected bool m_b5475_Keyboard_Program_Numeric_Mode = false;
        protected bool m_b5475_Keyboard_Program_Lower_Shift  = false;
        protected bool m_b5475_Error_Indicator_On            = false;
        protected bool m_b5475_LIO_Program_1_Indicator       = false;
        protected bool m_b5475_LIO_Program_2_Indicator       = false;

        // System/3-Defined SNS Status Bits
        protected bool m_b5475_SNS_1L_Lower_Shift_Key_Pressed       = false;
        protected bool m_b5475_SNS_1L_Invalid_Character_Detected    = false;

        protected bool m_b5475_SNS_2H_Program_1_Key_Pressed         = false;           
        protected bool m_b5475_SNS_2H_Program_2_Key_Pressed         = false;           
        protected bool m_b5475_SNS_2H_Release_Key_Pressed           = false;           
        protected bool m_b5475_SNS_2H_Field_Erase_Key_Pressed       = false;           
        protected bool m_b5475_SNS_2H_Error_Reset_Key_Pressed       = false;           
        protected bool m_b5475_SNS_2H_Read_Key_Pressed              = false;           

        protected bool m_b5475_SNS_2L_Record_Erase_Switch_Operated  = false;
        protected bool m_b5475_SNS_2L_Program_Switch_On             = false;
        protected bool m_b5475_SNS_2L_Skip_Key_Pressed              = false;
        protected bool m_b5475_SNS_2L_Dup_Key_Pressed               = false;
        protected bool m_b5475_SNS_2L_Auto_Record_Release_Switch_On = false;
                                                             
        protected bool m_b5475_SNS_3L_Keyboard_Interrupts_Enabled   = false;
        protected bool m_b5475_SNS_3L_Any_Function_Key_Pressed      = false;
        protected bool m_b5475_SNS_3L_Any_Data_Key                  = false;

        //const int k5475_SNS_1L_Print_Switch_On              = 0x0080;
        const int k5475_SNS_1L_Lower_Shift_Key_Pressed        = 0x0020;
        const int k5475_SNS_1L_Invalid_Character_Detected     = 0x0010;
        const int k5475_SNS_1L_Multi_Punch_Interrupt          = 0x0004;
        const int k5475_SNS_1L_Data_Key_Interrupt             = 0x0001;

        const int k5475_SNS_2H_Program_1_Key_Pressed          = 0x0080;           
        const int k5475_SNS_2H_Program_2_Key_Pressed          = 0x0040;           
        //const int k5475_SNS_2H_Program_Load_Switch_Operated = 0x0020;           
        const int k5475_SNS_2H_Release_Key_Pressed            = 0x0010;           
        const int k5475_SNS_2H_Field_Erase_Key_Pressed        = 0x0008;           
        const int k5475_SNS_2H_Error_Reset_Key_Pressed        = 0x0004;           
        const int k5475_SNS_2H_Read_Key_Pressed               = 0x0002;           
        //const int k5475_SNS_2H_Right_Adjust_Key_Pressed     = 0x0001;           

        //const int k5475_SNS_2L_Auto_Skip_Dup_Switch_On      = 0x0080;
        const int k5475_SNS_2L_Record_Erase_Switch_Operated   = 0x0040;
        const int k5475_SNS_2L_Program_Switch_On              = 0x0010;
        const int k5475_SNS_2L_Skip_Key_Pressed               = 0x0008;
        const int k5475_SNS_2L_Dup_Key_Pressed                = 0x0004;
        const int k5475_SNS_2L_Auto_Record_Release_Switch_On  = 0x0002;
        const int k5475_SNS_2L_Function_Key_Interrupt         = 0x0001;
                                                             
        const int k5475_SNS_3L_Keyboard_Interrupts_Enabled    = 0x0080;
        const int k5475_SNS_3L_Any_Function_Key_Pressed       = 0x0040;
        //const int k5475_SNS_3L_Bail_Forward_Contacts        = 0x0020;
        //const int k5475_SNS_3L_Unlock_Keyboard_Signal       = 0x0010;
        //const int k5475_SNS_3L_Bail_Forward_Trigger         = 0x0008;
        //const int k5475_SNS_3L_Toggle_Switch_Latch          = 0x0004;
        const int k5475_SNS_3L_Any_Data_Key                   = 0x0002;
        //const int k5475_SNS_3L_CE_Sense_Bit                 = 0x0001;

        // System/3-Defined SIO Control Code Bits
        const int k5475_SIO_0_Program_Numeric_Mode            = 0x80;
        const int k5475_SIO_1_Program_Lower_Shift             = 0x40;
        const int k5475_SIO_2_Set_Error_Indicator             = 0x20;
        const int k5475_SIO_4_Restore_Data_Key                = 0x08;
        const int k5475_SIO_5_Unlock_Data_Key                 = 0x04;
        const int k5475_SIO_6_Enable_Interrupt                = 0x02;
        const int k5475_SIO_7_Reset_Interrupt                 = 0x01;
#endregion

        // I/O device data - 5475 Data Entry Keyboard (one-card clock program test values)
        DateTime m_dtLast;
        int      m_iCount = 0,
                 m_iTotal = 0;
        //uint     m_uiMaxClockSpeed     = 0,
        //         m_uiCurrentClockSpeed = 0;
        //string   m_strCpuType = "";
        #region I/O device data - 5444 Disk Drive
        private CDiskControlField m_objDiskControlField = new CDiskControlField ();
        private CDumpData m_objDumpData             = new CDumpData ();
        private bool m_bDrive1Head0Selection        = true;
        private bool m_bDrive2Head0Selection        = true;
        private int  m_iDrive1Cylinder              = 0;
        private int  m_iDrive2Cylinder              = 0;
        //private int m_iDrive1FixedHeadSelected      = 0;
        //private int m_iDrive1RemovableHeadSelected  = 0;
        //private int m_iDrive2FixedHeadSelected      = 0;
        //private int m_iDrive2RemovableHeadSelected  = 0;
        private int m_iDiskControlAddressRegister   = 0;
        private int m_iDiskReadWriteAddressRegister = 0;
        private int m_iDiskStatusDrive1Bytes0and1 = m_ki5444Byte1Bit1CylinderZero;
        private int m_iDiskStatusDrive1Bytes2and3   = 0x0000;
        private int m_iDiskStatusDrive2Bytes0and1 = m_ki5444Byte1Bit1CylinderZero;
        private int m_iDiskStatusDrive2Bytes2and3   = 0x0000;
        private int m_iLastDiskSNS                  = 0;
        private enum EDiskStatusFlags
        {
            DISK_Busy       = 0x01,
            DISK_Scan_Found = 0x02  // Reset only by SIO
        };
        private int m_iDisk1Status = 0;
        private int m_iDisk2Status = 0;

        // Status bit definitions
        private const int m_ki5444Byte0Bit0NoOp                       = 0x8000;  // Reset by SNS that reads this bit
   //   private const int m_ki5444Byte0Bit1InterventionRequired       = 0x4000;  // Drive# specific
   //   private const int m_ki5444Byte0Bit2MissingAddressMarker       = 0x2000;  // Reset by SIO
   //   private const int m_ki5444Byte0Bit3EquipmentCheck             = 0x1000;  // Drive# specific
   //   private const int m_ki5444Byte0Bit4DataCheck                  = 0x0800;
        private const int m_ki5444Byte0Bit5NoRecordFound              = 0x0400;
   //   private const int m_ki5444Byte0Bit6TrackConditionCheck        = 0x0200;
        private const int m_ki5444Byte0Bit7SeekCheck                  = 0x0100;  // Drive# specific
        private const int m_ki5444Byte1Bit0ScanEqualHit               = 0x0080;
        private const int m_ki5444Byte1Bit1CylinderZero               = 0x0040;  // Drive# specific
        private const int m_ki5444Byte1Bit2EndOfCylinder              = 0x0020;
   //   private const int m_ki5444Byte1Bit3SeekBusy                   = 0x0010;  // Drive# specific
   //   private const int m_ki5444Byte1Bit4100Cylinder                = 0x0008;
   //   private const int m_ki5444Byte1Bit5Overrun                    = 0x0004;
        private const int m_ki5444Byte1Bit6StatusAddressA             = 0x0002;  // Reset by SIO; always 0
        private const int m_ki5444Byte1Bit7StatusAddressB             = 0x0001;  // Reset by SIO; 0 = drive 1, 1 = drive 2
        private const int m_ki5444Byte2Bit0Unsafe                     = 0x8000;  // Drive# specific
   //   private const int m_ki5444Byte2Bit1TimingAnalysisProgramLineA = 0x4000;
   //   private const int m_ki5444Byte2Bit2TimingAnalysisProgramLineB = 0x2000;
   //   private const int m_ki5444Byte2Bit3TimingAnalysisProgramLineC = 0x1000;
   //   private const int m_ki5444Byte2Bit4Index                      = 0x0800;  // Drive# specific
   //   private const int m_ki5444Byte2Bit5Settling                   = 0x0400;  // Drive# specific
   //   private const int m_ki5444Byte2Bit6CESenseBit                 = 0x0200;
   //   private const int m_ki5444Byte2Bit7Model6                     = 0x0100;
   //   private const int m_ki5444Byte3Bit0CESenseBit0                = 0x0080;
   //   private const int m_ki5444Byte3Bit1CESenseBit1                = 0x0040;
   //   private const int m_ki5444Byte3Bit2CESenseBit2                = 0x0020;
   //   private const int m_ki5444Byte3Bit3NotBitRingInhibit          = 0x0010;
   //   private const int m_ki5444Byte3Bit4StandardWriteTrigger       = 0x0008;
   //   private const int m_ki5444Byte3Bit5ConditionPriorityRequest   = 0x0004;
   //   private const int m_ki5444Byte3Bit6BitRing0                   = 0x0002;
   //   private const int m_ki5444Byte3Bit7NotCCRegisterPosition17    = 0x0001;

        // Const values
        private const int  SECTOR_SIZE           = 256;
        private const int  SECTORS_PER_CYLINDER  = 48;
        private const int  BYTES_PER_CYLINDER    = SECTOR_SIZE * SECTORS_PER_CYLINDER;
        private const bool FIXED_DRIVE           = true;
        private const bool REMOVABLE_DRIVE       = false;
        private const bool PRIMARY_DRIVE         = true;
        private const bool SECONDARY_DRIVE       = false;
        #endregion

        // I/O device objects
        //EKeyboard m_eKeyboard = EKeyboard.KEY_None;
        C5203LinePrinter m_obj5203LinePrinter = new C5203LinePrinter ();
        C5424MFCU        m_obj5424MFCU        = new C5424MFCU ();
        C5444DiskDrive   m_obj5444DiskDrive   = new C5444DiskDrive ();

        // Diagnostic variables
        string m_strSIODetails;
        string m_strLIODetails;
        string m_strInstructionAppendix = "";
        //List<string> strlSectorReading = new List<string> ();
        protected List<string> m_strlIOBuffer   = new List<string> (30);
        protected List<string> m_strlHplBuffer  = new List<string> (30);
        protected List<string> m_strl5475Buffer = new List<string> (30);

        #region Enumerations
        //public enum EInitialization
        //{
        //    INIT_None,
        //    INIT_Card,
        //    INIT_Disk,
        //};

        public enum EUIRunMode
        {
            RUN_Undefined,
            RUN_OutputToPrinterPanel,
            RUN_OutputPunchPattern,
            RUN_LoadFromIPL,
            RUN_LoadFromText,
            RUN_LoadFromDiskImage,
            RUN_EditScript,
            RUN_RunScript
        };

        public enum EBootDevice
        {
            BOOT_Card,
            BOOT_Disk_Fixed,
            BOOT_Disk_Removable
        };

        public enum EOutputTarget
        {
            OUTPUT_Unspecified,
            OUTPUT_Console,
            OUTPUT_DasmPanel,
            OUTPUT_TracePanel,
            OUTPUT_PrinterPanel,
            OUTPUT_5471Printer
        };

        enum ERegisterMask
        {
            REGMSK_IAR_IL  = 0x80,
            REGMSK_IAR_IL1 = 0x40,
            REGMSK_IAR_IL2 = 0x20,
            REGMSK_IAR_IL3 = 0x10,
            REGMSK_IAR_IL4 = 0x08,
            REGMSK_PL2IAR  = 0x40,
            REGMSK_PL1IAR  = 0x20,
            REGMSK_IAR     = 0x10,
            REGMSK_ARR     = 0x08,
            REGMSK_PSR     = 0x04,
            REGMSK_XR2     = 0x02,
            REGMSK_XR1     = 0x01
        };

        enum ERegister
        {
            REG_None    = 0,
            REG_IAR     = 1,
            REG_ARR     = 2,
            REG_XR1     = 3,
            REG_XR2     = 4,
            REG_PSR     = 5,
            REG_IAR_IL0 = 6,
            REG_IAR_IL1 = 7,
            REG_IAR_IL2 = 8,
            REG_IAR_IL3 = 9,
            REG_IAR_IL4 = 10,
            REG_PL1IAR  = 11,
            REG_PL2IAR  = 12
        };

        enum EIODevice
        {
            DEV_5410_CPU     = 0x00,
            DEV_5424_MFCU    = 0xF0,
            DEV_5203_Printer = 0xE0,
            DEV_5444_Disk_1  = 0xA0,
            DEV_5444_Disk_2  = 0xB0,
            DEV_Keyboard     = 0x10
        };

        public void SetUIRunMode (EUIRunMode eUIRunMode)
        {
            m_eUIRunMode = eUIRunMode;
        }

        public EUIRunMode GetUIRunMode ()
        {
            return m_eUIRunMode;
        }

        // Moved to Disassembler.cs
        // enum EKeyboard
        // {
        //     KEY_None,
        //     KEY_5471,
        //     KEY_5475
        // };
        #endregion

        // An event, delegate, and event handler args class must all be defined at the same time
        //public delegate void ENewXR1Handler (object sender, ENewXR1HandlerArgs ArgsXR1);
        //public event ENewXR1Handler ENewXR1 = new ENewXR1Handler (this, 0x0000);

        // Address Class
        protected class CTwoOperandAddress
        {
            int m_iOperandOneAddress = 0;
            int m_iOperandTwoAddress = 0;

            public CTwoOperandAddress (int iOperandOneAddress,
                                       int iOperandTwoAddress)
            {
                m_iOperandOneAddress = iOperandOneAddress;
                m_iOperandTwoAddress = iOperandTwoAddress;
            }

            public int OperandOneAddress
            {
                get { return m_iOperandOneAddress; }
            }

            public int OperandTwoAddress
            {
                get { return m_iOperandTwoAddress; }
            }
        }

        // Condition Register Class
        public enum ERegisterFlags
        {
            COND_Equal           = 0x01,
            COND_Low             = 0x02,
            COND_High            = 0x04,
            COND_DecimalOverflow = 0x08,
            COND_TestFalse       = 0x10,
            COND_BinaryOverflow  = 0x20,
            COND_AllFlags        = (COND_Equal | COND_Low | COND_High | COND_DecimalOverflow | COND_TestFalse | COND_BinaryOverflow) // 0x3F
        };

        protected class CConditionRegister
        {
            // IBM System/3 bit assignment
            // 0x80  0  Unassigned
            // 0x40  1  Unassigned
            // 0x20  2  Binary Underflow
            // 0x10  3  Test False
            // 0x08  4  Decimal Overflow
            // 0x04  5  High
            // 0x02  6  Low
            // 0x01  7  Equal

            public event EventHandler<NewCREventArgs> OnNewCR;

            static bool s_bInTest = false;
            byte m_yConditionRegister = (byte)ERegisterFlags.COND_Equal;

            public static void SetTestMode ()   { s_bInTest = true; }
            public static void ResetTestMode () { s_bInTest = false; }

            public void Load (int iPSRValue)
            {
                m_yConditionRegister = (byte)(iPSRValue & 0x3F);

                if ((m_yConditionRegister & ((byte)ERegisterFlags.COND_Equal |
                                             (byte)ERegisterFlags.COND_Low)) == 0x00)
                {
                    m_yConditionRegister |= (byte)ERegisterFlags.COND_High;
                }

                if (OnNewCR != null) { OnNewCR (this, new NewCREventArgs (m_yConditionRegister)); }
            }

            public int Store ()
            {
                return (short)(m_yConditionRegister & (byte)ERegisterFlags.COND_AllFlags);
            }

            public bool IsEqual ()
            {
                return (m_yConditionRegister & (byte)ERegisterFlags.COND_Equal) == (byte)ERegisterFlags.COND_Equal;
            }

            public bool IsLow ()
            {
                return (m_yConditionRegister & (byte)ERegisterFlags.COND_Low) == (byte)ERegisterFlags.COND_Low;
            }

            public bool IsHigh ()
            {
                return (m_yConditionRegister & (byte)ERegisterFlags.COND_High) == (byte)ERegisterFlags.COND_High;
            }

            public bool IsDecimalOverflow ()
            {
                return (m_yConditionRegister & (byte)ERegisterFlags.COND_DecimalOverflow) == (byte)ERegisterFlags.COND_DecimalOverflow;
            }

            public bool IsTestFalse ()
            {
                bool bIsTestFalse = (m_yConditionRegister & (byte)ERegisterFlags.COND_TestFalse) == (byte)ERegisterFlags.COND_TestFalse;

                return (m_yConditionRegister & (byte)ERegisterFlags.COND_TestFalse) == (byte)ERegisterFlags.COND_TestFalse;
            }

            public bool IsBinaryOverflow ()
            {
                return (m_yConditionRegister & (byte)ERegisterFlags.COND_BinaryOverflow) == (byte)ERegisterFlags.COND_BinaryOverflow;
            }

            public void SystemReset ()
            {
                m_yConditionRegister = (byte)ERegisterFlags.COND_Equal;
            }

            public void SetEqual ()
            {
                // Reset High, Low, and Equal
                m_yConditionRegister &= ((byte)ERegisterFlags.COND_DecimalOverflow |
                                         (byte)ERegisterFlags.COND_TestFalse       |
                                         (byte)ERegisterFlags.COND_BinaryOverflow);

                // Set only Equal flag
                m_yConditionRegister |= (byte)ERegisterFlags.COND_Equal;

                if (OnNewCR != null) { OnNewCR (this, new NewCREventArgs (m_yConditionRegister)); }
            }

            public void SetLow ()
            {
                // Reset High, Low, and Equal
                m_yConditionRegister &= ((byte)ERegisterFlags.COND_DecimalOverflow |
                                         (byte)ERegisterFlags.COND_TestFalse       |
                                         (byte)ERegisterFlags.COND_BinaryOverflow);

                // Set only Low flag
                m_yConditionRegister |= (byte)ERegisterFlags.COND_Low;

                if (OnNewCR != null) { OnNewCR (this, new NewCREventArgs (m_yConditionRegister)); }
            }

            public void SetHigh ()
            {
                // Reset High, Low, and Equal
                m_yConditionRegister &= ((byte)ERegisterFlags.COND_DecimalOverflow |
                                         (byte)ERegisterFlags.COND_TestFalse       |
                                         (byte)ERegisterFlags.COND_BinaryOverflow);

                // Set only High flag
                m_yConditionRegister |= (byte)ERegisterFlags.COND_High;

                if (OnNewCR != null) { OnNewCR (this, new NewCREventArgs (m_yConditionRegister)); }
            }

            public void SetDecimalOverflow ()
            {
                m_yConditionRegister |= (byte)ERegisterFlags.COND_DecimalOverflow;

                if (OnNewCR != null) { OnNewCR (this, new NewCREventArgs (m_yConditionRegister)); }
            }

            public void SetTestFalse ()
            {
                m_yConditionRegister |= (byte)ERegisterFlags.COND_TestFalse;

                if (OnNewCR != null) { OnNewCR (this, new NewCREventArgs (m_yConditionRegister)); }
            }

            public void SetBinaryOverflow ()
            {
                m_yConditionRegister |= (byte)ERegisterFlags.COND_BinaryOverflow;

                if (OnNewCR != null) { OnNewCR (this, new NewCREventArgs (m_yConditionRegister)); }
            }

            public void TestResetDecimalOverflow ()
            {
                if (s_bInTest)
                {
                    m_yConditionRegister &= ((byte)ERegisterFlags.COND_Equal     |
                                             (byte)ERegisterFlags.COND_Low       |
                                             (byte)ERegisterFlags.COND_High      |
                                             (byte)ERegisterFlags.COND_TestFalse |
                                             (byte)ERegisterFlags.COND_BinaryOverflow);

                    if (OnNewCR != null) { OnNewCR (this, new NewCREventArgs (m_yConditionRegister)); }
                }
            }

            public void TestResetTestFalse ()
            {
                if (s_bInTest)
                {
                    m_yConditionRegister &= ((byte)ERegisterFlags.COND_Equal           |
                                             (byte)ERegisterFlags.COND_Low             |
                                             (byte)ERegisterFlags.COND_High            |
                                             (byte)ERegisterFlags.COND_DecimalOverflow |
                                             (byte)ERegisterFlags.COND_BinaryOverflow);

                    if (OnNewCR != null) { OnNewCR (this, new NewCREventArgs (m_yConditionRegister)); }
                }
            }

            public void ResetBinaryOverflow ()
            {
                m_yConditionRegister &= ((byte)ERegisterFlags.COND_Equal           |
                                         (byte)ERegisterFlags.COND_Low             |
                                         (byte)ERegisterFlags.COND_High            |
                                         (byte)ERegisterFlags.COND_DecimalOverflow |
                                         (byte)ERegisterFlags.COND_TestFalse);

                if (OnNewCR != null) { OnNewCR (this, new NewCREventArgs (m_yConditionRegister)); }
            }

            private void ResetDecimalOverflow ()
            {
                m_yConditionRegister &= ((byte)ERegisterFlags.COND_Equal     |
                                         (byte)ERegisterFlags.COND_Low       |
                                         (byte)ERegisterFlags.COND_High      |
                                         (byte)ERegisterFlags.COND_TestFalse |
                                         (byte)ERegisterFlags.COND_BinaryOverflow);

                if (OnNewCR != null) { OnNewCR (this, new NewCREventArgs (m_yConditionRegister)); }
            }

            private void ResetTestFalse ()
            {
                m_yConditionRegister &= ((byte)ERegisterFlags.COND_Equal           |
                                         (byte)ERegisterFlags.COND_Low             |
                                         (byte)ERegisterFlags.COND_High            |
                                         (byte)ERegisterFlags.COND_DecimalOverflow |
                                         (byte)ERegisterFlags.COND_BinaryOverflow);

                if (OnNewCR != null) { OnNewCR (this, new NewCREventArgs (m_yConditionRegister)); }
            }

            public bool IsNoOp (byte yQByte)
            {
                // NoOp: 80, X7, XF (X = 0 - 7); 0x80 || (yQByte & 0x07)
                if (yQByte == 0x80 ||
                    (yQByte & 0x87) == 0x07)
                {
                    // NoOp
                    return true;
                }

                return false;
            }

            // This should ONLY be called by JC and BC !!!
            public bool IsConditionTrue (byte yQByte)
            {
                // First test for unconditional jump/branch execution
                // Unconditional: 00, X7, XF (X = 8 - F); 0x00 || (yQByte & 0x87) == 0x87
                if (yQByte == 0x00 ||
                    (yQByte & 0x87) == 0x87)
                {
                    // Unconditional branch
                    return true;
                }

                // Reset Test False and Decimal Overflow if tested here
                bool bConditionMet = false;
                if ((yQByte & 0x80) == 0x80)
                {
                    //(yQByte & 0x80) = 0x80: branch if ANY tested condition is true
                    if ((yQByte & m_yConditionRegister) > 0)
                    {
                        bConditionMet = true;
                    }
                }
                else
                {
                    //(yQByte & 0x80) = 0x00: branch only if ALL tested conditions are false        }
                    if ((yQByte & m_yConditionRegister) == 0)
                    {
                        // Condition met; do the branch
                        bConditionMet = true;
                    }
                }

                // Reset Test False if tested
                if ((yQByte & (byte)ERegisterFlags.COND_TestFalse) == (byte)ERegisterFlags.COND_TestFalse)
                {
                    ResetTestFalse ();
                }

                // Reset Decimal Overflow if tested
                if ((yQByte & (byte)ERegisterFlags.COND_DecimalOverflow) == (byte)ERegisterFlags.COND_DecimalOverflow)
                {
                    ResetDecimalOverflow ();
                }

                return bConditionMet;
            }
        };

        #region Breakpoint Support
        protected enum EBreakpoint
        {
            POINT_none,
            POINT_IAR,
            POINT_IAR_Count,
            POINT_Data_Read,
            POINT_Data_Read_Value,
            POINT_Data_Read_Count,
            POINT_Data_Write,
            POINT_Data_Write_Value,
            POINT_Reg_Read,
            POINT_Reg_Write,
            POINT_Op_Code,
            POINT_QByte,
            POINT_Control_Code
        };

        protected enum EPointType
        {
            PT_none,
            PT_Breakpoint,
            PT_TraceTrigger
        };

        protected class CBreakpoint
        {
            //bool        m_bEnabled    = true;
            //short       m_iAddress    = 0;
            //short       m_iCount      = 0;
            //short       m_iDataValue  = 0x00;
            //EBreakpoint m_eBreakpoint = EBreakpoint.POINT_none;
            //ERegister   m_eRegister   = ERegister.REG_None;
        };

        Dictionary<int, CBreakpoint> m_dictBreakpoints = new Dictionary<int,CBreakpoint> ();
        #endregion

        #region Event-Firing Methods
        public void FireMakeRegisterLabelsDormantEvent ()
        {
            if (m_bAsyncRun)
            {
                lock (m_objLock)
                {
                    OnMakeRegisterLabelsDormant?.Invoke (this, new EventArgs ());
                }
            }
        }

        public void FireMakeRegisterLabelsActiveEvent ()
        {
            if (m_bAsyncRun)
            {
                lock (m_objLock)
                {
                    OnMakeRegisterLabelsActive?.Invoke (this, new EventArgs ());
                }
            }
        }

        public void FireUpdateAppTitleEvent ()
        {
            if (m_bAsyncRun)
            {
                lock (m_objLock)
                {
                    OnUpdateAppTitle?.Invoke (this, new EventArgs ());
                }
            }
        }

        public void FireResetControlColorsEvent ()
        {
            if (m_bAsyncRun)
            {
                lock (m_objLock)
                {
                    OnResetControlColors?.Invoke (this, new EventArgs ());
                }
            }
        }

        public void FireShowTraceStateEvent ()
        {
            if (m_bAsyncRun)
            {
                lock (m_objLock)
                {
                    OnShowTraceState?.Invoke (this, new EventArgs ());
                }
            }
        }

        public void FireResetTraceListEvent ()
        {
            if (m_bAsyncRun)
            {
                lock (m_objLock)
                {
                    OnResetTraceList?.Invoke (this, new EventArgs ());
                }
            }
        }

        public void FireInitializeTracePanelEvent ()
        {
            if (m_bAsyncRun)
            {
                lock (m_objLock)
                {
                    OnInitializeTracePanel?.Invoke (this, new EventArgs ());
                }
            }
        }

        public void FireProgramStateEvent (string strProgrameState)
        {
            if (m_bAsyncRun)
            {
                lock (m_objLock)
                {
                    OnNewProgramState?.Invoke (this, new NewProgramStateEventArgs (strProgrameState));
                }
            }
        }

        public void FireProcessorCheckEvents (string str1, string str2)
        {
            if (m_bAsyncRun)
            {
                lock (m_objLock)
                {
                    OnNewProcessorCheck1?.Invoke (this, new NewProcessorCheck1EventArgs (str1));
                    OnNewProcessorCheck2?.Invoke (this, new NewProcessorCheck2EventArgs (str2));
                }
            }
        }

        public void FireClearHaltAnd5475Events ()
        {
            if (m_bAsyncRun)
            {
                lock (m_objLock)
                {
                    OnNewHaltCode?.Invoke (this, new NewHaltCodeEventArgs (""));
                    OnNew5475Code?.Invoke (this, new New5475CodeEventArgs (""));
                }
            }
        }

        public void FireClearPrinterPanelEvent ()
        {
            if (m_bAsyncRun)
            {
                lock (m_objLock)
                {
                    OnClearPrintOutputPanel?.Invoke (this, new EventArgs ());
                }
            }
        }

        public void FireClearGrayedLinesListEvent ()
        {
            if (m_bAsyncRun)
            {
                OnClearGrayedLinesList?.Invoke (this, new EventArgs ());
            }
        }

        public void FirePrintToPrinterEvent (string strPrinLine)
        {
            if (m_bAsyncRun)
            {
                OnNewPrinterString?.Invoke (this, new NewPrinterStringEventArgs (strPrinLine + Environment.NewLine));
            }
        }

        public void FireNewCPUDialsEvent (Int16 iCPUDials, bool bEnable)
        {
            if (m_bAsyncRun)
            {
                OnNewCPUDials?.Invoke (this, new NewCPUDialsEventArgs (iCPUDials, bEnable));
            }
        }

        public void FireNewDASMStringListEvent (List<string> lstrDasmStrings)
        {
            if (m_bAsyncRun)
            {
                OnNewDASMStringList?.Invoke (this, new NewDASMStringListEventArgs (lstrDasmStrings));
            }
        }

        public void FireNewTraceStringListEvent (List<string> lstrTraceStrings)
        {
            if (m_bAsyncRun)
            {
                OnNewTraceStringList?.Invoke (this, new NewTraceStringListEventArgs (lstrTraceStrings));
            }
        }

        public void FireNewPrinterStringListEvent (List<string> lstrPrinterStrings)
        {
            if (m_bAsyncRun)
            {
                OnNewPrinterStringList?.Invoke (this, new NewPrinterStringListEventArgs (lstrPrinterStrings));
            }
        }

        public void FireOnNewEnableHighlightLineEvent (bool bEnable)
        {
            if (m_bAsyncRun)
            {
                OnNewEnableHighlightLine?.Invoke (this, new NewEnableHighlightLineEventArgs (bEnable));
            }
        }

        public void FireNewDisassemblyListingEvent (int iBeginDasmAddress, int iEndDasmAddress, int iDasmEntryPoint, int iXR1, int iXR2)
        {
            m_bNewDisassemblyListingEventCalled = true;
            if (m_bAsyncRun)
            {
                OnNewDisassemblyListing?.Invoke (this, new NewDisassemblyEventArgs (iBeginDasmAddress, iEndDasmAddress, iDasmEntryPoint, iXR1, iXR2));
            }
        }

        public void FireIPLProgramSizeEvent (int iProgramSize)
        {
            if (m_bAsyncRun)
            {
                lock (m_objLock)
                {
                    OnIPLProgramSize?.Invoke (this, new NewProgramSizeEventArgs (iProgramSize));
                }
            }
        }
        #endregion

        #region System Initialization
        public static string GetDBPath ()
        {
            if (File.Exists (DATABASE_PATH_DEBUG))
            {
                return DATABASE_PATH_DEBUG;
            }
            else if (File.Exists (DATABASE_PATH_RELEASE))
            {
                return DATABASE_PATH_RELEASE;
            }

            MessageBox.Show ("Can't find Symulator3.accdb", "Symulator/3", MessageBoxButton.OK, MessageBoxImage.Error);
            
            return "";
        }

        public CEmulatorEngine (bool bAsync = false)
        {
            m_bAsyncRun        = bAsync;
            m_objEmulatorState = new CEmulatorState (this);
            m_objPrintQueue    = new CPrintQueue (this);
            m_objTraceQueue    = new CTraceQueue (this);
            m_bAutoTagJump     = true;
            m_bAutoTagLoop     = true;

            //m_eKeyboard        = EKeyboard.KEY_5475;
            //DBInit (DATABASE_PATH);
            //m_obj5444DiskDrive.DBInit (DATABASE_PATH);
            string strDBPath = GetDBPath ();
            if (strDBPath.Length == 0)
            {
                Application.Current.Shutdown ();
            }
            else
            {
                DBInit (strDBPath);
                m_obj5444DiskDrive.DBInit (strDBPath);

                m_obj5424MFCU.NameOutputFiles ();
            }
        }

        public void SetMemorySize (int iMemorySizeInK)
        {
            m_iMemorySizeInK = iMemorySizeInK;
            m_bMemorySizeSet = true;
        }

        public void SetSystemDate (string strSystemDate )
        {
            m_strSystemDate  = strSystemDate;
            m_bSystemDateSet = true;
        }

        //public void SetPrinterChainImage (string strPrinterChainImage)
        //{
        //    m_strPrinterChainImage  = strPrinterChainImage;
        //    m_bPrinterChainImageSet = true;
        //}

        public void SetCopyright (string strCopyright)
        {
            m_strCopyright  = strCopyright;
            m_bCopyrightSet = true;
        }
#endregion

#region EventHandlers
        public event EventHandler<NewIAREventArgs>                 OnNewIAR;
        public event EventHandler<NewARREventArgs>                 OnNewARR;
        public event EventHandler<NewXR1EventArgs>                 OnNewXR1;
        public event EventHandler<NewXR2EventArgs>                 OnNewXR2;
        public event EventHandler<NewCREventArgs>                  OnNewCR;
        public event EventHandler<NewLPFLREventArgs>               OnNewLPFLR;
        public event EventHandler<NewLPIAREventArgs>               OnNewLPIAR;
        public event EventHandler<NewLPDAREventArgs>               OnNewLPDAR;
        public event EventHandler<NewMPDAREventArgs>               OnNewMPDAR;
        public event EventHandler<NewMRDAREventArgs>               OnNewMRDAR;
        public event EventHandler<NewMUDAREventArgs>               OnNewMUDAR;
        public event EventHandler<NewDCAREventArgs>                OnNewDCAR;
        public event EventHandler<NewDRWAREventArgs>               OnNewDRWAR;
        public event EventHandler<NewStepCountEventArgs>           OnNewStepCount;
        public event EventHandler<NewCPUDialsEventArgs>            OnNewCPUDials;
        public event EventHandler<NewEnableHighlightLineEventArgs> OnNewEnableHighlightLine;
        public event EventHandler<NewDisassemblyEventArgs>         OnNewDisassemblyListing;
        public event EventHandler<NewDASMStringEventArgs>          OnNewDASMString;
        public event EventHandler<NewDASMStringListEventArgs>      OnNewDASMStringList;
        public event EventHandler<NewTraceStringEventArgs>         OnNewTraceString;
        public event EventHandler<NewTraceStringListEventArgs>     OnNewTraceStringList;
        public event EventHandler<New5471StringEventArgs>          OnNew5471String;
        public event EventHandler<NewPrinterStringEventArgs>       OnNewPrinterString;
        public event EventHandler<NewPrinterStringListEventArgs>   OnNewPrinterStringList;
        public event EventHandler<NewHaltCodeEventArgs>            OnNewHaltCode;
        public event EventHandler<New5475CodeEventArgs>            OnNew5475Code;
        public event EventHandler<NewProgramStateEventArgs>        OnNewProgramState;
        public event EventHandler<NewProcessorCheck1EventArgs>     OnNewProcessorCheck1;
        public event EventHandler<NewProcessorCheck2EventArgs>     OnNewProcessorCheck2;
        public event EventHandler<NewProgramSizeEventArgs>         OnIPLProgramSize;
        public event EventHandler<EventArgs>                       OnClearPrintOutputPanel;
        public event EventHandler<EventArgs>                       OnClearGrayedLinesList;
        public event EventHandler<EventArgs>                       OnResetControlColors;
        public event EventHandler<EventArgs>                       OnResetTraceList;
        public event EventHandler<EventArgs>                       OnShowTraceState;
        public event EventHandler<EventArgs>                       OnInitializeTracePanel;
        public event EventHandler<EventArgs>                       OnMakeRegisterLabelsDormant;
        public event EventHandler<EventArgs>                       OnMakeRegisterLabelsActive;
        public event EventHandler<EventArgs>                       OnUpdateAppTitle;
#endregion

        public void SystemReset (bool bClearPrinterList = false, bool bCalledFromProgramLoad = false)
        {
            m_iIL      = IL_Main;
            m_iaIAR[0] = 0x0000;
            m_iaIAR[1] = 0x0000;
            m_iaIAR[2] = 0x0000;
            m_iaIAR[3] = 0x0000;
            m_iaIAR[4] = 0x0000;
            m_iaIAR[5] = 0x0000;
            m_iIARpl1  = 0x0000;
            m_iIARpl2  = 0x0000;
            m_iaARR[0] = 0x0000;
            m_iaARR[1] = 0x0000;
            m_iaARR[2] = 0x0000;
            m_iaARR[3] = 0x0000;
            m_iaARR[4] = 0x0000;
            m_iaARR[5] = 0x0000;
            m_iARRpl1  = 0x0000;
            m_iARRpl2  = 0x0000;
            m_iXR1     = 0x0000;
            m_iXR2     = 0x0000;

            m_iOldStartDASMAddress  = 0x0000;
            m_iOldEndDASMAddress    = CARD_IMAGE_IPL_SIZE;
            m_iOldDASMEntryPoint    = 0x0000;
            m_iNewStartDASMAddress  = 0x0000;
            m_iNewEndDASMAddress    = 0x0000;
            m_iNewDASMEntryPoint    = 0x0000;
            m_iInstructionCount     = 0;
            m_iIPLCardCount         = 0;
            m_iIPLCardReadCount     = 0;
            m_bIsAbsoluteCardLoader = false;
            m_bNewDisassemblyListingEventCalled = false;

            m_strKeyStroke = "";
            m_cKeyStroke   = '\x00';
            m_bKeyAlt      = false;
            m_bKeyCtrl     = false;

            m_eKeyboard = EKeyboard.KEY_None;
            if (!bCalledFromProgramLoad)
            {
                m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_1_NoProgramLoaded); // CEmulatorState m_eProgramState = EProgramState.STATE_Stopped;
            }

            OnNewIAR?.Invoke         (this, new NewIAREventArgs       (m_iaIAR[IL_Main], m_iIL)); // Null conditional operator (?.) new in C# 6.0 / VS2015
            OnNewARR?.Invoke         (this, new NewARREventArgs       (m_iaARR[IL_Main], m_iIL));
            OnNewXR1?.Invoke         (this, new NewXR1EventArgs       (m_iXR1));
            OnNewXR2?.Invoke         (this, new NewXR2EventArgs       (m_iXR2));
            OnNewLPFLR?.Invoke       (this, new NewLPFLREventArgs     (m_obj5203LinePrinter.FormLength));
            OnNewLPIAR?.Invoke       (this, new NewLPIAREventArgs     (m_obj5203LinePrinter.i5203ChainImageAddressRegister));
            OnNewLPDAR?.Invoke       (this, new NewLPDAREventArgs     (m_obj5203LinePrinter.i5203PrintDAR));
            OnNewMPDAR?.Invoke       (this, new NewMPDAREventArgs     (m_obj5424MFCU.i5424PrintDAR));
            OnNewMRDAR?.Invoke       (this, new NewMRDAREventArgs     (m_obj5424MFCU.i5424ReadDAR));
            OnNewMUDAR?.Invoke       (this, new NewMUDAREventArgs     (m_obj5424MFCU.i5424PunchDAR));
            OnNewDCAR?.Invoke        (this, new NewDCAREventArgs      (m_iDiskControlAddressRegister));
            OnNewDRWAR?.Invoke       (this, new NewDRWAREventArgs     (m_iDiskReadWriteAddressRegister));
            OnNewStepCount?.Invoke   (this, new NewStepCountEventArgs (m_iInstructionCount));
            OnShowTraceState?.Invoke (this, new EventArgs             ());
            OnResetTraceList?.Invoke (this, new EventArgs             ());
            OnNewCPUDials?.Invoke    (this, new NewCPUDialsEventArgs  ((short)m_usConsoleDialSetting, true));

            //if (OnNew5471String != null)
            //{
            //    OnNew5471String (this, new New5471StringEventArgs ("<---------------------------------- New 5471 string ---------------------------------->"));
            //}

            OnNewHaltCode?.Invoke (this, new NewHaltCodeEventArgs (""));
            OnNew5475Code?.Invoke (this, new New5475CodeEventArgs (""));

            foreach (CConditionRegister cr in m_aCR)
            {
                cr.SystemReset ();
            }

            if (OnNewCR != null) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store (), true)); }

            m_obj5203LinePrinter.ResetUnprintableCharacter ();
            Reset5475Sns001 ();
            Reset5475Sns010 ();
            Reset5475Sns011 ();
            Reset5475StatusFlags ();

            m_obj5424MFCU.ClearPrimaryHopper ();
            m_obj5424MFCU.ClearSecondaryHopper ();

            if (bClearPrinterList)
            {
                m_objPrintQueue.Clear ();
            }
        }

        public void Initialize (EBootDevice eBootDevice)
        {
            if (m_bClearMemory)
            {
                // Clear system data area
                for (int iIdx = 0x0100; iIdx < 0x0200; iIdx++)
                {
                    m_yaMainMemory[iIdx] = 0x00;
                }

                // Set memory high address
                m_yaMainMemory[0x017E] = 0xFF; // Set memory size at 64k
                SetMemorySize (m_yaMainMemory.Length);

                // Set printer chain image 0x0100 - 0x0177
                if (eBootDevice == EBootDevice.BOOT_Card)
                {
                    m_eKeyboard = EKeyboard.KEY_5475;
                    m_bKeyboardInputEnabled = false;
                    //m_i5203LineWidth = 96;
                    m_obj5203LinePrinter.SetPrinterWidth96 ();
                    //m_i5203ChainImageAddressRegister = 0x0100;
                    m_obj5203LinePrinter.i5203ChainImageAddressRegister = 0x0100;
                    if (m_objEmulatorState.IsSingleStep () && OnNewLPIAR != null) { OnNewLPIAR (this, new NewLPIAREventArgs (m_obj5203LinePrinter.i5203ChainImageAddressRegister)); }
                    for (int iSAR = 0x0100; iSAR < 0x0178; iSAR++)
                    {
                        m_yaMainMemory[iSAR] = 0x40;
                    }
                    LoadBinaryImage (m_ya48CharacterPrinterChainImage, 0x0100, 0);
                    m_obj5203LinePrinter.ChainImage = ConvertEbcdicToAscii (m_ya48CharacterPrinterChainImage);
                }
                else if (eBootDevice == EBootDevice.BOOT_Disk_Fixed ||
                         eBootDevice == EBootDevice.BOOT_Disk_Removable)
                {
                    m_eKeyboard = EKeyboard.KEY_5471;
                    m_bKeyboardInputEnabled = true;
                    //m_i5203LineWidth = 132;
                    m_obj5203LinePrinter.SetPrinterWidth132 ();
                    //m_i5203ChainImageAddressRegister = 0x0400;
                    m_obj5203LinePrinter.i5203ChainImageAddressRegister = 0x0400;
                    if (m_objEmulatorState.IsSingleStep () && OnNewLPIAR != null) { OnNewLPIAR (this, new NewLPIAREventArgs (m_obj5203LinePrinter.i5203ChainImageAddressRegister)); }
                    for (int iSAR = 0x0400; iSAR < 0x0478; iSAR++)
                    {
                        m_yaMainMemory[iSAR] = 0x40;
                    }
                    LoadBinaryImage (m_ya48CharacterPrinterChainImage, 0x0400, 0);
                    m_obj5203LinePrinter.ChainImage = ConvertEbcdicToAscii (m_ya48CharacterPrinterChainImage);
                }

                //StringBuilder strbldrChainImage = new StringBuilder (120);
                //for (int iIdx = 0; iIdx < 120; iIdx++)
                //{
                //    strbldrChainImage.Append ((char)ConvertEBCDICtoASCII (m_ya48CharacterPrinterChainImage[iIdx % m_ya48CharacterPrinterChainImage.Length]));
                //}
                //StringBuilder strbldrChainImage = new StringBuilder (m_ya48CharacterPrinterChainImage.Length);
                //foreach (byte yEbcdic in m_ya48CharacterPrinterChainImage)
                //{
                //    strbldrChainImage.Append ((char)ConvertEBCDICtoASCII (yEbcdic));
                //    //strbldrChainImage.Append ((char)m_ya48CharacterPrinterChainImage[iIdx]);
                //}
                //m_obj5203LinePrinter.ChainImage = strbldrChainImage.ToString ();
                //SetPrinterChainImage (strbldrChainImage.ToString ());

                // Set printer default values
                m_obj5203LinePrinter.FormLength = 66;

                // Set system date 0x0178 - 0x017D
                DateTime dtNow  = DateTime.Now;
                string strMonth = string.Format ("{0:D2}", dtNow.Month);
                string strDay   = string.Format ("{0:D2}", dtNow.Day);
                string strYear  = string.Format ("{0:D4}", dtNow.Year);

                m_yaMainMemory[0x0178] = (byte)ConvertTopTierCharacter (strMonth[0]);
                m_yaMainMemory[0x0179] = (byte)ConvertTopTierCharacter (strMonth[1]);
                m_yaMainMemory[0x017A] = (byte)ConvertTopTierCharacter (strDay[0]);
                m_yaMainMemory[0x017B] = (byte)ConvertTopTierCharacter (strDay[1]);
                m_yaMainMemory[0x017C] = (byte)ConvertTopTierCharacter (strYear[2]);
                m_yaMainMemory[0x017D] = (byte)ConvertTopTierCharacter (strYear[3]);

                // Set copyright 0x01B2
                for (int iSAR = 0x01B2; iSAR < 0x01B2 + 46; iSAR++)
                {
                    m_yaMainMemory[iSAR] = 0x40;
                }

                string strCopyright = "COPYRIGHT (C) " + strYear + " SACRED CAT SOFTWARE";
                for (int iIdx = 0; iIdx < strCopyright.Length; iIdx++)
                {
                    m_yaMainMemory[0x01B2 + iIdx] = (byte)ConvertTopTierCharacter (strCopyright[iIdx]);
                }
            }
        }

        public void ClearPrimaryHopper ()
        {
            m_obj5424MFCU.ClearPrimaryHopper ();
        }

        public void ClearSecondaryHopper ()
        {
            m_obj5424MFCU.ClearSecondaryHopper ();
        }

        public bool IsPrimaryHopperEmpty ()
        {
            return m_obj5424MFCU.IsPrimaryHopperEmpty ();
        }

        public bool IsSecondaryHopperEmpty ()
        {
            return m_obj5424MFCU.IsSecondaryHopperEmpty ();
        }

        public void LoadPrimaryHopperBlankCard ()
        {
            m_obj5424MFCU.LoadPrimaryHopperBlankCard ();
        }

        public void LoadSecondaryHopperBlankCard ()
        {
            m_obj5424MFCU.LoadSecondaryHopperBlankCard ();
        }

        public void AssignFileToPrimaryHopper (string strFilename, C5424MFCU.ELoadHopperMode eLoadHopperMode = C5424MFCU.ELoadHopperMode.LOAD_OnlyIfEmpty)
        {
            m_obj5424MFCU.LoadPrimaryHopperFile (strFilename, eLoadHopperMode);
        }

        public void AssignTokenToPrimaryHopper (string strToken, C5424MFCU.ELoadHopperMode eLoadHopperMode = C5424MFCU.ELoadHopperMode.LOAD_OnlyIfEmpty)
        {
            List<string> lstrListFromToken = ReadListFromToken (strToken);
            m_obj5424MFCU.LoadPrimaryHopperFile (lstrListFromToken, eLoadHopperMode);
        }

        public void AssignFileToSecondaryHopper (string strFilename, C5424MFCU.ELoadHopperMode eLoadHopperMode = C5424MFCU.ELoadHopperMode.LOAD_OnlyIfEmpty)
        {
            m_obj5424MFCU.LoadSecondaryHopperFile (strFilename, eLoadHopperMode);
        }

        public void AssignTokenToSecondaryHopper (string strToken, C5424MFCU.ELoadHopperMode eLoadHopperMode = C5424MFCU.ELoadHopperMode.LOAD_OnlyIfEmpty)
        {
            List<string> lstrListFromToken = ReadListFromToken (strToken);
            m_obj5424MFCU.LoadSecondaryHopperFile (lstrListFromToken, eLoadHopperMode);
        }

        public void ResetHPLDisplay ()
        {
            if (m_iHPL_IL == m_iIL)
            {
                OnNewHaltCode?.Invoke (this, new NewHaltCodeEventArgs (""));
                m_iHPL_IL = -1;
            }
        }

        public void ProgramLoadCard (string strCardFilename, bool bClearWaitStations = false, int iTraceTriggerIAR = 0x00010000)
        {
            m_iTraceTriggerIAR = iTraceTriggerIAR;
            m_obj5424MFCU.SetClearWaitStations ();
            ProgramLoad (EBootDevice.BOOT_Card, strCardFilename, "", "");
        }

        public void ProgramLoadCard (EDatabaseTable eDatabaseTable, string strCardFilename, bool bClearWaitStations = false, int iTraceTriggerIAR = 0x00010000)
        {
            m_iTraceTriggerIAR = iTraceTriggerIAR;
            m_obj5424MFCU.SetClearWaitStations ();
            ProgramLoad (EBootDevice.BOOT_Card, strCardFilename, "", "", eDatabaseTable);
        }

        public int ProgramLoadText (string strProgramToken, ref int riStartAddress, ref int riEndAddress, ref int riEntryPoint, ref byte[] rlyEndCardImage)
        {
            if (strProgramToken.Length == 0 ||
                !m_sdFileTokens.ContainsKey (strProgramToken))
            {
                return 0; // lyProgramImage.ToArray ();
            }

            riStartAddress = 0xFFFF;
            riEndAddress   = 0x0000;
            riEntryPoint   = 0xFFFF;

            //m_i5203LineWidth = 96;
            m_obj5203LinePrinter.SetPrinterWidth96 ();

            // Load all card images into string list
            List<string> lstrListFromToken = ReadListFromToken (strProgramToken);
            int iRecordCount = lstrListFromToken.Count;

            LoadTextFile (lstrListFromToken);
            m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_2_ProgramLoaded);

            return iRecordCount;
        }

        public void ProgramLoadCardDasm (string strCardFilename, int iTrigger, int iStart, int iEnd, int iEntryPoint,
                                         bool bStartInData = true, bool bRunAfterDASM = false)
        {
            if (iEnd > iStart)
            {
                m_iDasmTriggerAddress = iTrigger;
                m_iDasmStartAddress   = iStart;
                m_iDasmEndAddress     = iEnd;
                m_bRunAfterDASM       = bRunAfterDASM;
                m_iEntryPoint         = iEntryPoint;
                if (bStartInData)
                {
                    m_bInData = (m_iDasmStartAddress < m_iEntryPoint);
                    m_bInDataFlagSet = true;
                }
            }

            ProgramLoad (EBootDevice.BOOT_Card, strCardFilename, "", "");

            ResetTriggeredDasm ();
        }

        public void ProgramLoadDisk (EBootDevice eBootDevice, string strFixedDriveFilename, string strRemovableDriveFilename)
        {
            ProgramLoad (eBootDevice, "", strFixedDriveFilename, strRemovableDriveFilename);
        }

        public void ProgramLoad (EBootDevice eBootDevice, string strCardFilename, string strFixedDriveFilename, string strRemovableDriveFilename,
                                 EDatabaseTable eDatabaseTable = EDatabaseTable.TABLE_Undefined)
        {
            DateTime dtNow = DateTime.Now;
            SetLogFileame (string.Format ("{0} {1:D4}-{2:D2}-{3:D2}-{4:D2}-{5:D2}-{6:D2}.txt", m_strTraceLogFilenamePrefix,
                                          dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second));
            m_eBootDevice = eBootDevice;
            SystemReset ();
            Initialize (eBootDevice);

            if (m_eBootDevice == EBootDevice.BOOT_Card)
            {
                //m_i5203LineWidth = 96;
                m_obj5203LinePrinter.SetPrinterWidth96 ();

                if (eDatabaseTable == EDatabaseTable.TABLE_Undefined)
                {
                    if (strCardFilename.Length > 0)
                    {
                        m_obj5424MFCU.LoadPrimaryHopperFile (strCardFilename);
                        byte[] yaBootCardImage = m_obj5424MFCU.ReadCardFromPrimaryIPL ();
                        LoadBinaryImage (yaBootCardImage, 0x0000, 0);
                    }
                }
                else
                {
                    //List<string> lstrCardFile = ReadCardFileToStringList (eDatabaseTable, "", strCardFilename);
                    //byte[] yaBootCardImage = m_obj5242MFCU.ReadCardFromPrimaryIPL ();
                    //LoadBinaryImage (yaBootCardImage, 0x0000, 0);
                }
            }
            else if (m_eBootDevice == EBootDevice.BOOT_Disk_Fixed ||
                     m_eBootDevice == EBootDevice.BOOT_Disk_Removable)
            {
                //m_i5203LineWidth = 132;
                m_obj5203LinePrinter.SetPrinterWidth132 ();

                if (strFixedDriveFilename.Length < 1 ||
                    !File.Exists (strFixedDriveFilename))
                {
                    WaveRedFlag ("File " + strFixedDriveFilename + " not found");
                    m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_1_NoProgramLoaded);
                    return;
                }

                if (strFixedDriveFilename.Length > 0)
                {
                    m_obj5444DiskDrive.LoadDiskImageFromFile (strFixedDriveFilename, FIXED_DRIVE, PRIMARY_DRIVE);
                }

                if (strRemovableDriveFilename.Length > 0)
                {
                    m_obj5444DiskDrive.LoadDiskImageFromFile (strRemovableDriveFilename, REMOVABLE_DRIVE, PRIMARY_DRIVE);
                }

                byte[] yaBootSectorImage = new byte[1]; // Minimal load for throw-away variable

                if (m_eBootDevice == EBootDevice.BOOT_Disk_Fixed)
                {
                    yaBootSectorImage = m_obj5444DiskDrive.ReadFixedDriveBootSector ();
                }
                else if (m_eBootDevice == EBootDevice.BOOT_Disk_Removable)
                {
                    yaBootSectorImage = m_obj5444DiskDrive.ReadRemovableDriveBootSector ();
                }

                //PrintStringList (DisassembleCodeImage (yaBootSectorImage, 0x0000, 0x0100)); // Boot sector
                LoadBinaryImage (yaBootSectorImage, 0x0000, 0);
            }
            else
            {
                // No boot device specified
            }

            // Begin executing program beginning at 0x0000
            m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_2_ProgramLoaded);
            DoRun (0x0000);

            if ((m_eBootDevice == EBootDevice.BOOT_Disk_Fixed      ||
                 m_eBootDevice == EBootDevice.BOOT_Disk_Removable) &&
                !m_bDebugDiskIPL)
            {
                m_obj5444DiskDrive.WriteAllDirtyImages ();
            }
        }

        public int ProgramLoadDB (string strFirstToken, string strSecondToken = "", bool bBootSecondary = false, bool bRunImmediately = true)
        {
            if (strFirstToken.Length == 0 ||
                !m_sdFileTokens.ContainsKey (strFirstToken))
            {
                return 0;
            }

            CDBFileToken ftOne = m_sdFileTokens[strFirstToken];
            CDBFileToken ftTwo = (m_sdFileTokens.ContainsKey (strSecondToken)) ? m_sdFileTokens[strSecondToken] : null;

            ETokenType ettOne = ftOne.GetTokenType ();
            ETokenType ettTwo = (ftTwo == null) ? ETokenType.TOKEN_Invalid : ftTwo.GetTokenType ();

            m_iProgramSizeIPL = 0;

            //Primary must be TOKEN_CardBinary or TOKEN_DiskBinary
            if (ettOne != ETokenType.TOKEN_CardBinary &&
                ettOne != ETokenType.TOKEN_DiskBinary)
            {
                return 0;
            }

            //Secondary must not be TOKEN_InvalidFilePath
            if (ettTwo == ETokenType.TOKEN_InvalidFilePath)
            {
                return 0;
            }

            //If Primary is TOKEN_CardBinary, secondary must not be TOKEN_DiskBinary
            if (ettOne == ETokenType.TOKEN_CardBinary &&
                ettTwo == ETokenType.TOKEN_DiskBinary)
            {
                return 0;
            }

            //If Primary is TOKEN_DiskBinary, secondary must be TOKEN_DiskBinary or TOKEN_ValidFilePath
            if (ettOne == ETokenType.TOKEN_DiskBinary &&
                ettTwo != ETokenType.TOKEN_DiskBinary &&
                ettTwo != ETokenType.TOKEN_ValidFilePath)
            {
                return 0;
            }

            DateTime dtNow = DateTime.Now;
            SetLogFileame (string.Format ("{0} {1:D4}-{2:D2}-{3:D2}-{4:D2}-{5:D2}-{6:D2}.txt", m_strTraceLogFilenamePrefix,
                                          dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second));

            m_eBootDevice = ettOne == ETokenType.TOKEN_CardBinary ? EBootDevice.BOOT_Card           :
                                      bBootSecondary              ? EBootDevice.BOOT_Disk_Removable : EBootDevice.BOOT_Disk_Fixed;
            SystemReset (false, true);
            Initialize (m_eBootDevice);
            int iRecordCount = 0;

            if (m_eBootDevice == EBootDevice.BOOT_Card)
            {
                //m_i5203LineWidth = 96;
                m_obj5203LinePrinter.SetPrinterWidth96 ();

                List<string> lstrListFromToken1 = ReadListFromToken (strFirstToken);
                iRecordCount = lstrListFromToken1.Count;
                m_obj5424MFCU.LoadPrimaryHopperFile (lstrListFromToken1);
                byte[] yaBootCardImage = m_obj5424MFCU.ReadCardFromPrimaryIPL ();
                LoadBinaryImage (yaBootCardImage, 0x0000, 0);
                m_iIPLCardReadCount = 1;

                if (ettTwo == ETokenType.TOKEN_CardBinary ||
                    ettTwo == ETokenType.TOKEN_CardData)
                {
                    List<string> lstrListFromToken2 = ReadListFromToken (strSecondToken);
                    iRecordCount = lstrListFromToken2.Count;
                    m_obj5424MFCU.LoadSecondaryHopperFile (lstrListFromToken2);
                }
            }
            else if (m_eBootDevice == EBootDevice.BOOT_Disk_Fixed ||
                     m_eBootDevice == EBootDevice.BOOT_Disk_Removable)
            {
                //m_i5203LineWidth = 132;
                m_obj5203LinePrinter.SetPrinterWidth132 ();

                // Load Primary Fixed drive from first token
                if (!m_obj5444DiskDrive.LoadDiskImageFromToken (ftOne, FIXED_DRIVE, PRIMARY_DRIVE))
                {
                    return 0;
                }

                // Load Primary Removable drive from second token or file
                if (ettTwo == ETokenType.TOKEN_DiskBinary)
                {
                    if (!m_obj5444DiskDrive.LoadDiskImageFromToken (ftTwo, REMOVABLE_DRIVE, PRIMARY_DRIVE))
                    {
                        return 0;
                    }
                }
                else if (ettTwo == ETokenType.TOKEN_ValidFilePath)
                {
                    if (!m_obj5444DiskDrive.LoadDiskImageFromFile (ftTwo.FilePath, REMOVABLE_DRIVE, PRIMARY_DRIVE))
                    {
                        return 0;
                    }
                }

                byte[] yaBootSectorImage = new byte[1]; // Minimal load for throw-away variable

                if (m_eBootDevice == EBootDevice.BOOT_Disk_Fixed)
                {
                    yaBootSectorImage = m_obj5444DiskDrive.ReadFixedDriveBootSector ();
                }
                else if (m_eBootDevice == EBootDevice.BOOT_Disk_Removable)
                {
                    yaBootSectorImage = m_obj5444DiskDrive.ReadRemovableDriveBootSector ();
                }

                //PrintStringList (DisassembleCodeImage (yaBootSectorImage, 0x0000, 0x0100)); // Boot sector
                LoadBinaryImage (yaBootSectorImage, 0x0000, 0);
            }
            else
            {
                // No boot device specified
            }

            // Begin executing program beginning at 0x0000
            m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_2_ProgramLoaded);
            m_iTraceTriggerIAR = 0x0000;

            if (bRunImmediately)
            {
                DoRun (0x0000);
            }

            if ((m_eBootDevice == EBootDevice.BOOT_Disk_Fixed      ||
                 m_eBootDevice == EBootDevice.BOOT_Disk_Removable) &&
                !m_bDebugDiskIPL)
            {
                m_obj5444DiskDrive.WriteAllDirtyImages ();
            }

            return iRecordCount;
        }

        //byte[] m_yaPrntObjectImage = { 0x7C, 0x00, 0x52, 0x75, 0x02, 0xCE, 0x71, 0xF5,
        //                               0xCE, 0xF3, 0xF9, 0x07, 0xD1, 0xF9, 0x0C, 0x6D,
        //                               0x01, 0xD2, 0x01, 0xD0, 0x81, 0xB3, 0x7C, 0xBC,
        //                               0x1C, 0x9D, 0x00, 0x00, 0xBC, 0xF2, 0x81, 0x0D,
        //                               0x5E, 0x00, 0x1C, 0x3A, 0x7D, 0xC2, 0x1C, 0xD0,
        //                               0x82, 0x19, 0xF2, 0x00, 0x0B, 0x7C, 0x06, 0x37,
        //                               0x5E, 0x00, 0x37, 0x1C, 0x9C, 0x00, 0x00, 0x06,
        //                               0xE2, 0x02, 0x01, 0x74, 0x02, 0xE0, 0x7D, 0x60,
        //                               0xE0, 0xD0, 0x82, 0x16, 0x7C, 0xC7, 0x81, 0x7C,
        //                               0xB9, 0x50, 0x7C, 0x20, 0x51, 0x75, 0x02, 0xCE,
        //                               0xB9, 0x20, 0x00, 0xBC, 0x40, 0x9C, 0xF2, 0x90,
        //                               0x03, 0xBC, 0x5C, 0x9C, 0xE2, 0x02, 0x01, 0x74,
        //                               0x02, 0xE0, 0x79, 0x1F, 0xE0, 0xD0, 0x90, 0x50,
        //                               0x71, 0xE6, 0xD0, 0xF3, 0xE2, 0x01, 0xD1, 0xE6,
        //                               0x6E, 0x7D, 0x10, 0x51, 0xF2, 0x86, 0x03, 0x7C,
        //                               0xB8, 0x50, 0x5E, 0x00, 0x81, 0x3A, 0x5C, 0x00,
        //                               0x51, 0xC7, 0x7D, 0xCD, 0x81, 0xD0, 0x82, 0x4D,
        //                               0x5E, 0x00, 0x52, 0x4B, 0x7D, 0x60, 0x52, 0xD0,
        //                               0x82, 0x44, 0x70, 0xE0, 0xE0, 0x7D, 0x30, 0xDF,
        //                               0xF2, 0x82, 0x09, 0xF3, 0xE4, 0x8C, 0xD1, 0xE4,
        //                               0x9E, 0xD0, 0x00, 0x00, 0xF3, 0xE0, 0x03, 0xD1,
        //                               0xE4, 0xA7, 0xF3, 0xE0, 0x03, 0xD1, 0xE4, 0xAD,
        //                               0xD0, 0x00, 0x00, 0xF3, 0xE4, 0x01, 0xF0, 0x7C,
        //                               0x63, 0xD0, 0x00, 0xB6, 0x40, 0x50, 0x60, 0x61,
        //                               0xD0, 0xF0, 0x30, 0x2A, 0x10, 0x21, 0x00, 0x20,
        //                               0x10, 0x08, 0x04, 0x02, 0x01, 0x04, 0x00, 0x04,
        //                               0x7C, 0x61, 0x5C };

        public void Stop ()
        {
            //Console.WriteLine ("public void Stop ()");
            if (m_objEmulatorState.IsRunning ())
            {
                m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_5_Aborted);
            }
            m_bStopRun = true;
        }

        public void UnloadProgram ()
        {
            m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_1_NoProgramLoaded);
            FireOnNewEnableHighlightLineEvent (false);
            SystemReset (true);
        }

        public void DoRun (int iStartAddress = 0x0000, bool bStartAlways = false)
        {
            if (bStartAlways ||
                m_objEmulatorState.GetProgramState () == CEmulatorState.EProgramState.PSTATE_2_ProgramLoaded)
            {
                if (m_bAsyncRun)
                {
                    DoRunAsync (iStartAddress);
                }
                else
                {
                    RunProgram (iStartAddress);
                }
            }
        }

        private async void DoRunAsync (int iStartAddress)
        {
            await RunAsync (iStartAddress);
        }

        public Task RunAsync (int iStartAddress)
        {
            return Task.Run (() => { RunProgram (iStartAddress); });
        }

        private void RunProgram (int iStartAddress = 0x0000)
        {
            if (!m_objEmulatorState.IsProgramLoaded ())
            {
                return;
            }

            m_objTraceQueue.UpdateOnly ();
            SetIsInEmulator (true);
            m_bStopRun              = false;
            m_bMFCUBufferRulerShown = false;
            if (m_bAsyncRun)
            {
                FireInitializeTracePanelEvent ();
            }

            // Variables defined for DEBUG_DISK_IPL
            CTraceParser objTraceParser = null;
            List<string> strlErrors = new List<string> ();

            if ((m_eBootDevice == EBootDevice.BOOT_Disk_Fixed ||
                 m_eBootDevice == EBootDevice.BOOT_Disk_Removable))
            {
                m_usConsoleDialSetting = 0x5471; // Tell SCP to use console for user input

                if (m_bDebugDiskIPL)
                {
                    objTraceParser = new CTraceParser ();
                    objTraceParser.LoadTraceLog (@"D:\SoftwareDev\IBMSystem3\System3Simulator\trace_new.log");
                }
            }

            // Begin program execution beginning at specified address
            m_iaIAR[IL_Main]    = iStartAddress;
            if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[IL_Main], IL_Main)); }
            m_iInstructionCount = 0;
            if (m_objEmulatorState.IsSingleStep () && OnNewStepCount != null) { OnNewStepCount (this, new NewStepCountEventArgs (m_iInstructionCount)); }
            m_iHaltCount        = 0;
            ulong ulTotalTiming = 0;
            int   iTraceLogIdx  = 0;

#if LOCAL_UNIT_TESTS
            byte[] yaLIO = { 0x71, 0xF5, 0x2A };
            byte[] yaMVI = { 0x7C, 0x00, 0x40 };
            byte[] yaCLI = { 0x7D, 0xFF, 0x40 };
            int iOp1Start = -1,
                iOp1End   = -1,
                iOp2Start = -1,
                iOp2End   = -1,
                iOverlap  = -1,
                iTiming   = -1;
            bool bValidInstruction = ComputeAddresses (yaLIO, ref iOp1Start, ref iOp1End, ref iOp2Start, ref iOp2End, ref iOverlap, ref iTiming);
            bValidInstruction = ComputeAddresses (yaMVI, ref iOp1Start, ref iOp1End, ref iOp2Start, ref iOp2End, ref iOverlap, ref iTiming);
            bValidInstruction = ComputeAddresses (yaCLI, ref iOp1Start, ref iOp1End, ref iOp2Start, ref iOp2End, ref iOverlap, ref iTiming);

            Console.WriteLine (FormatShortDataString (0x0100, 0x0101));
            Console.WriteLine (FormatShortDataString (0x0100, 0x0106));
            Console.WriteLine (FormatShortDataString (0x0100, 0x0107));

            Console.WriteLine (FormatDataString (0x0100, 0x010E));
            Console.WriteLine (FormatDataString (0x0100, 0x0115));
            Console.WriteLine (FormatDataString (0x0105, 0x0110, true));

            for (int iIdx = 0; iIdx < 0x00FF; ++iIdx)
                m_yaMainMemory[iIdx] = (byte)iIdx;

            List<string> strlDumpRAM = BinaryToDump (m_yaMainMemory, 0x0000, 0x00FF);
            foreach (string str in strlDumpRAM)
                Console.WriteLine (str);

            Console.WriteLine ("");
            Console.WriteLine (FormatDataString (0x0001, 0x000F, true));

            Console.WriteLine ("");
            Console.WriteLine (FormatDataString (0x0007, 0x000F, true));

            // Test case: both 7 bytes
            Console.WriteLine ("");
            List<string> strlBefore = FormatDataContents (0x0000, 0x0006, 0x0010, 0x0016);
            foreach (string str in strlBefore)
                Console.WriteLine (str);

            // Test case: both 8 bytes
            Console.WriteLine ("");
            strlBefore = FormatDataContents (0x0000, 0x0007, 0x0010, 0x0017);
            foreach (string str in strlBefore)
                Console.WriteLine (str);

            // Test case: both 9 bytes
            Console.WriteLine ("");
            strlBefore = FormatDataContents (0x0000, 0x0008, 0x0010, 0x0018);
            foreach (string str in strlBefore)
                Console.WriteLine (str);

            // Test case: one 7 bytes, on 9 bytes
            Console.WriteLine ("");
            strlBefore = FormatDataContents (0x0000, 0x0006, 0x0010, 0x0018);
            foreach (string str in strlBefore)
                Console.WriteLine (str);

            // Test case: both 15 bytes
            Console.WriteLine ("");
            strlBefore = FormatDataContents (0x0000, 0x000E, 0x0010, 0x001E);
            foreach (string str in strlBefore)
                Console.WriteLine (str);

            // Test case: both 16 bytes
            Console.WriteLine ("");
            strlBefore = FormatDataContents (0x0000, 0x000F, 0x0010, 0x001F);
            foreach (string str in strlBefore)
                Console.WriteLine (str);

            // Test case: both 17 bytes, one on 0x0010 boundary, the other not
            Console.WriteLine ("");
            strlBefore = FormatDataContents (0x0040, 0x0050, 0x0041, 0x005E);
            foreach (string str in strlBefore)
                Console.WriteLine (str);

            // Test case: both 65 bytes not on boundary
            Console.WriteLine ("");
            strlBefore = FormatDataContents (0x0005, 0x0045, 0x0008, 0x0048);
            foreach (string str in strlBefore)
                Console.WriteLine (str);
#endif

            if ((m_eBootDevice == EBootDevice.BOOT_Disk_Fixed      ||
                 m_eBootDevice == EBootDevice.BOOT_Disk_Removable) &&
                 m_bDebugDiskIPL                                   &&
                 //m_objEmulatorState.IsSingleStep ()                &&
                 IsInTrace ())
            {
                string strRegisters = string.Format ("    Initial register values:      XR1: {0:X4}  XR2: {1:X4}  ARR: {2:X4}  CR: {3:S}",
                                                     m_iXR1, m_iXR2, m_iaARR[m_iIL], GetConditionFlags (m_aCR[m_iIL]));
                WriteOutput (strRegisters, EOutputTarget.OUTPUT_TracePanel);
            }

            m_objEmulatorState.SetRunState ();
            while (m_objEmulatorState.IsRunning ())
            {
                ++m_iInstructionCount;
                if (m_objEmulatorState.IsSingleStep () && OnNewStepCount != null) { OnNewStepCount (this, new NewStepCountEventArgs (m_iInstructionCount)); }
                if (m_bLimitInstructionCount &&
                    m_iInstructionCount >= m_iInstructionCountLimit)
                {
                    m_objEmulatorState.SetPaused ();
                    break;
                }

                //if ((m_eBootDevice == EBootDevice.BOOT_Disk_Fixed ||
                //     m_eBootDevice == EBootDevice.BOOT_Disk_Removable) &&
                //     m_bDebugDiskIPL)
                //{
                //    if (833500 == m_iInstructionCount)
                //    {
                //        SetTrace ();
                //        SetShowDisassembly ();
                //        SetShowChangedValues ();
                //        SetShowDiskBuffers ();
                //        SetDelayInstructions ();
                //        SetLogToFile ();
                //    }
                //}

                if (m_iaIAR[m_iIL] == 0x0019) // Address of BC to exit the End card image
                {
                    //SetTrace ();
                    //SetShowDisassembly ();
                    //SetShowChangedValues ();
                    //SetShowMFCUBuffers ();
                }

                if (m_bLimitInstructionCount &&
                    m_iHaltCount >= m_iHaltCountLimit)
                {
                    m_objEmulatorState.SetPaused (); // m_eProgramState = EProgramState.STATE_Paused;
                    break;
                }

                // Code to intercept execution after loading
                // 0x007C  0x0060  0x00FF  Branch into Absolute Card Loader
                // 0x00DB  0x0000  0x005F  Completion of one 4-byte to 3-byte compression pass
                // 0x0019  0x0100  0x01FF  Branch into end card instructions
                // 0x0019  0x0019  0x0057  End card
                // 0x0AA5  0x0D56  0x13FF  Main program
                if (m_iDasmTriggerAddress < 0x00010000)
                {
                    if (m_iaIAR[m_iIL] == m_iDasmTriggerAddress)
                    {
                        m_bLoadingComplete = true;
                        m_bDasmCalledByEmu = true;
                        SetIsInEmulator (false);
                        //WriteOutputStringList (DisassembleCodeFromEmulator (m_yaMainMemory, m_iXR1, m_iXR2), EOutputTarget.OUTPUT_TracePanel);
                        m_objTraceQueue.EnqueueTraceLines (DisassembleCodeFromEmulator (m_yaMainMemory, m_iXR1, m_iXR2));
                        m_bDasmCalledByEmu = false;
                        ResetTriggeredDasm ();
                        if (!m_bRunAfterDASM)
                        {
                            return;
                        }
                        SetIsInEmulator (true);
                    }
                }

                if (m_iTraceTriggerIAR < 0x00010000)
                {
                    if (m_iaIAR[m_iIL] == m_iTraceTriggerIAR &&
                        m_objEmulatorState.IsSingleStep ())
                    {
                        SetTrace ();
                        SetShowDisassembly ();
                        SetShowChangedValues ();
                        SetShowIOBuffers ();
                        m_iTraceTriggerIAR = 0x00010000;
                        FireShowTraceStateEvent ();
                    }
                }

                try
                {
                    m_objTraceQueue.UpdateAndOutputToTracePanel ();
                    BreakOrContinue ();
                }
                catch (Exception e)
                {
                    if (e.Message == ESCAPE_KEY)
                    {
                        SetIsInEmulator (false);

                        return;
                    }
                }

                // Check for breakpoints
                //if (m_dictBreakpoints.Count > 0 &&
                //    m_dictBreakpoints.ContainsKey (m_iaIAR[m_iIL]))
                //{
                //    // Handle breakpoint
                //    CBreakpoint bkp = m_dictBreakpoints[m_iaIAR[m_iIL]];
                //}

                // Save interrupt level for current instruction
                int iPrevIL = m_iIL;

                //// Check for keyboard input
                //if (m_bKeyboardInputEnabled &&
                //    !m_bAsyncRun            &&
                //    Console.KeyAvailable)
                //{
                HandleKeyboardInput ();
                //}

                // Reset printer busy & set Printer Interrupt Pending
                if (m_iInstructionCount == m_iResetBusyCount)
                {
                    m_i5471StatusFlags &= ~STAT_Prt5471_Printer_Busy;
                    m_i5471StatusFlags |= STAT_Prt5471_Printer_Interrupt_Pending;
                    m_iIL = IL_1_Keyboard;
                }

                int iIAR = m_iaIAR[m_iIL];
                byte[] yaInstruction = FetchInstruction ();
                int iInstructionTiming = ComputeTiming (yaInstruction);
                ulTotalTiming += (ulong)iInstructionTiming;
                string strInstruction = "";
                List<string> strlBefore = new List<string> ();

                if (m_bStopRun)
                {
                    Console.WriteLine ("if (m_bStopRun)");
                    m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_5_Aborted);
                    m_bStopRun = false;
                    return;
                }

                if (m_objEmulatorState.IsSingleStep () /*&&
                    IsInTrace ()*/)
                {
                    if (m_iInstructionCount == 153271) // ?? Boot from disk?  Stop just before prompt for DATE?
                    {
                        return;
                    }
                    List<string> strlInstruction = DisassembleCodeTrace (yaInstruction, m_iXR1, m_iXR2, iIAR);
                    strInstruction = FormatInstructionLine (strlInstruction[0], yaInstruction, iIAR);
                    if (GetShowDisassembly ())
                    {
                        //WriteOutput ("", EOutputTarget.OUTPUT_TracePanel);
                        m_objTraceQueue.EnqueueTraceLine ("");
                        if (m_bDebugDiskIPL)
                        {
                            //WriteOutput (string.Format ("Step: {0} <{1}>, Timing: {2:###,###,###,###}.{3:00}{4}s ({5})",
                            //                             m_iInstructionCount, iTraceLogIdx + 1,
                            //                             ulTotalTiming / 100, ulTotalTiming % 100,
                            //                             MICRO_SIGN, FormatTotalTime (ulTotalTiming)));
                            WriteOutput (string.Format ("Step: {0} <{1}>, Timing: {2} ({3})",
                                                         m_iInstructionCount, iTraceLogIdx + 1,
                                                         FormatTotalCPUTime (ulTotalTiming), FormatTotalTime (ulTotalTiming)), EOutputTarget.OUTPUT_TracePanel);
                        }
                        else
                        {
                            //WriteOutput (string.Format ("Step: {0}, Timing: {1:###,###,###,###}.{2:00}{3}s ({4})",
                            //                              m_iInstructionCount, ulTotalTiming / 100, ulTotalTiming % 100,
                            //                              MICRO_SIGN, FormatTotalTime (ulTotalTiming)));
                            //WriteOutput (string.Format ("Step: {0}, {1}Timing: {2} ({3})",
                            m_objTraceQueue.EnqueueTraceLine (string.Format ("Step: {0}, {1}Timing: {2} ({3})",
                                                              m_iInstructionCount, new string (' ', 7 - m_iInstructionCount.ToString ().Length),
                                                              FormatTotalCPUTime (ulTotalTiming),
                                                              FormatTotalTime (ulTotalTiming))); // , EOutputTarget.OUTPUT_TracePanel);
                        }

                        string strIL = "IL: ";
                        if (m_iIL == IL_Main)
                        {
                            strIL += "M";
                        }
                        else
                        {
                            strIL += (char)('0' + m_iIL - 1);
                        }
                        m_strInstructionAppendix += string.Format ("  Timing: {0}.{1:00}{2}s", iInstructionTiming / 100, iInstructionTiming % 100, MICRO_SIGN);

                        foreach (string str in strlInstruction)
                        {
                            //WriteOutput (strIL + str.Substring (19), EOutputTarget.OUTPUT_TracePanel);
                            m_objTraceQueue.EnqueueTraceLine (strIL + str.Substring (19));
                            strIL = "     ";
                        }
                    }

                    if (GetShowChangedValues ())
                    {
                        if (InstructionReadsRAM (yaInstruction[0]) || InstructionWritesRAM (yaInstruction[0]))
                        {
                            int iOp1Start = -1,
                                iOp1End   = -1,
                                iOp2Start = -1,
                                iOp2End   = -1,
                                iOverlap  = -1,
                                iTiming   = -1;
                            if (ComputeAddresses (yaInstruction, ref iOp1Start, ref iOp1End, ref iOp2Start, ref iOp2End,
                                                  ref iOverlap, ref iTiming, m_iXR1, m_iXR2))
                            {
                                strlBefore = FormatDataContents (iOp1Start, iOp1End, iOp2Start, iOp2End);
                            }
                        }
                    }
                }

                try
                {
                    if (m_objEmulatorState.IsSingleStep () && OnResetControlColors != null) { OnResetControlColors (this, new EventArgs ()); }
                    ResetHPLDisplay ();
                    ExecuteInstruction (yaInstruction);
                    if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[IL_Main], IL_Main)); }
                }
                catch (Exception e)
                {
                    //Console.WriteLine (e.StackTrace);
                    if (e.Message == ESCAPE_KEY)
                    {
                        SetIsInEmulator (false);

                        return;
                    }
                }

                if (m_bSimulateSystem3CpuTiming &&
                    !m_bTestOneCardClockProgram)
                {
                    if (m_iInstructionDelayCount < 0)
                    {
                        m_iInstructionDelayCount = SYSTEM_3_CPU_TIME_DELAY;
                    }
                    
                    if (--m_iInstructionDelayCount == 0)
                    {
                        m_iInstructionDelayCount = SYSTEM_3_CPU_TIME_DELAY;
                        Thread.Sleep (2);
                    }
                }

                if ((m_eBootDevice == EBootDevice.BOOT_Disk_Fixed ||
                     m_eBootDevice == EBootDevice.BOOT_Disk_Removable) &&
                     m_bDebugDiskIPL)
                {
                    if (//m_objEmulatorState.IsSingleStep () &&
                        IsInTrace ())
                    {
                        strlErrors = objTraceParser.AnalyzeLine (m_iaARR, iIAR, m_iXR1, m_iXR2, strInstruction.Substring (6, 4),
                                                                 m_iDiskReadWriteAddressRegister, m_iDiskControlAddressRegister,
                                                                 m_iLastDiskSNS, m_objDiskControlField, ref m_iIL, ref iTraceLogIdx);

                        if (iPrevIL == IL_Main &&
                            m_iIL   == IL_1_Keyboard)
                        {
                            // Switching to IL1
                            // Reset Printer Busy flag
                            m_i5471StatusFlags &= ~STAT_Prt5471_Printer_Busy;

                            // Set Printer Interrupt Pending flag
                            m_i5471StatusFlags |= STAT_Prt5471_Printer_Interrupt_Pending;
                        }

                        if (objTraceParser.IsAbort ())
                        {
                            m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_5_Aborted);
                        }
                    }
                }

                if (m_objEmulatorState.IsSingleStep () &&
                    //IsInTrace () &&
                    strInstruction.Length > 0)
                {
                    // Also: IO registers depending on device
                    //       RAM used by & affected by instruction
                    //       RAM depending on device & operation
                    if (GetShowDisassembly ())
                    {
                        //WriteOutput (string.Format ("{0:S}{1:S}IAR: {2:X4}  XR1: {3:X4}  XR2: {4:X4}  ARR: {5:X4}  CR: {6:S} {7:S}",
                        //                             strInstruction, strInstruction.Length < 43 ? new string (' ', 43 - strInstruction.Length) : "",
                        //                             m_iaIAR[iPrevIL], m_iXR1, m_iXR2, m_iaARR[iPrevIL],
                        //                             GetConditionFlags (m_aCR[iPrevIL]), m_strInstructionAppendix), EOutputTarget.OUTPUT_TracePanel);
                        m_objTraceQueue.EnqueueTraceLine (string.Format ("{0:S}{1:S}IAR: {2:X4}  XR1: {3:X4}  XR2: {4:X4}  ARR: {5:X4}  CR: {6:S} {7:S}",
                                                          strInstruction, strInstruction.Length < 43 ? new string (' ', 43 - strInstruction.Length) : "",
                                                          m_iaIAR[iPrevIL], m_iXR1, m_iXR2, m_iaARR[iPrevIL],
                                                          GetConditionFlags (m_aCR[iPrevIL]), m_strInstructionAppendix));
                    }
                    //else
                    //{
                    //    WriteOutput (string.Format ("{0:S}IAR: {1:X4}  XR1: {2:X4}  XR2: {3:X4}  ARR: {4:X4}  IL: {5}  CR: {6:S} {7:S}",
                    //                                  strInstruction.Length < 43 ? new string (' ', 43 - strInstruction.Length) : "",
                    //                                  m_iaIAR[iPrevIL], m_iXR1, m_iXR2, m_iaARR[iPrevIL],
                    //                                  ((iPrevIL > 0) ? (char)('0' + iPrevIL - 1) : (char)'M'),
                    //                                  GetConditionFlags (m_aCR[iPrevIL]), m_strInstructionAppendix), EOutputTarget.OUTPUT_TracePanel);
                    //}

                    if (//IsInTrace ()                       &&
                        GetShowChangedValues ()            &&
                        m_objEmulatorState.IsSingleStep () &&
                        m_strLIODetails != null            &&
                        m_strLIODetails.Length > 0)
                    {
                        //WriteOutput (m_strLIODetails, EOutputTarget.OUTPUT_TracePanel);
                        m_objTraceQueue.EnqueueTraceLine (m_strLIODetails);
                        m_strLIODetails = "";
                    }

                    if (GetShowChangedValues ())
                    {
                        //WriteOutputStringList (strlBefore, EOutputTarget.OUTPUT_TracePanel, 0, true);
                        m_objTraceQueue.EnqueueTraceLines (strlBefore);

                        if (InstructionWritesRAM (yaInstruction[0]))
                        {
                            int iOp1Start = -1,
                                iOp1End   = -1,
                                iOp2Start = -1,
                                iOp2End   = -1,
                                iOverlap  = -1,
                                iTiming   = -1;
                            if (ComputeAddresses (yaInstruction, ref iOp1Start, ref iOp1End, ref iOp2Start, ref iOp2End,
                                                  ref iOverlap, ref iTiming, m_iXR1, m_iXR2))
                            {
                                if (iOverlap <= 0)
                                {
                                    iOp2Start = -1;
                                    iOp2End   = -1;
                                }
                                List<string> strlAfter = FormatDataContents (iOp1Start, iOp1End, iOp2Start, iOp2End);
                                //WriteOutputStringList (strlAfter, EOutputTarget.OUTPUT_TracePanel, 0, true);
                                m_objTraceQueue.EnqueueTraceLines (strlAfter);
                            }
                        }
                    }
                }

                //if (m_bCpuMemTesting)
                //{
                //    if (m_strInstructionAppendix.IndexOf ("Col 61-64:") > 0)
                //    {
                //        if (!IsInTrace ())
                //            WriteOutput (m_strInstructionAppendix, EOutputTarget.OUTPUT_TracePanel);
                //        WriteOutputStringList (DisassembleCodeImage (m_yaMainMemory, 0x0003, CARD_IMAGE_IPL_SIZE), EOutputTarget.OUTPUT_TracePanel, 0);
                //    }
                //}

                m_strInstructionAppendix = "";
                if (m_strSIODetails != null &&
                    m_strSIODetails.Length > 0)
                {
                    //WriteLogOutputLine (string.Format ("      {0:S}", m_strSIODetails));
                    //WriteOutput (string.Format ("      {0:S}", m_strSIODetails), EOutputTarget.OUTPUT_TracePanel);
                    m_objTraceQueue.EnqueueTraceLine (string.Format ("      {0:S}", m_strSIODetails));
                    m_strSIODetails = null;

                    if (//m_objEmulatorState.IsSingleStep () &&
                        IsInTrace ()                         &&
                        GetShowChangedValues ())
                    {
                        m_objDiskControlField.DisplayStates ();
                        DisplayStatus ();
                    }
                }

                if (GetShowMFCUBuffers ()    &&
                    m_strlIOBuffer.Count > 0 &&
                    !m_objEmulatorState.IsFreeRun ())
                {
                    //WriteOutputStringList (m_strlIOBuffer, EOutputTarget.OUTPUT_TracePanel, 0, true);
                    m_objTraceQueue.EnqueueMFCULines (m_strlIOBuffer);
                    m_strlIOBuffer.Clear ();
                }

                //if (GetShow5475Halt () &&
                //    m_strlHplBuffer.Count > 0)
                //{
                //    if (//m_objEmulatorState.IsSingleStep () &&
                //        (IsInTrace () ||
                //         !m_bFix7SegmentDisplay))
                //    {
                //        WriteOutputStringList (m_strlHplBuffer, EOutputTarget.OUTPUT_TracePanel, 0, true);
                //    }
                //    else
                //    {
                //        Print7SegmentList (m_strlHplBuffer, 0, 0, false, true);
                //    }
                //}

                //if (GetShow5475Halt () &&
                //    m_strl5475Buffer.Count > 0)
                //{
                //    if (//m_objEmulatorState.IsSingleStep () &&
                //        (IsInTrace () ||
                //         !m_bFix7SegmentDisplay))
                //    {
                //        WriteOutputStringList (m_strl5475Buffer, EOutputTarget.OUTPUT_TracePanel, 0, true);
                //    }
                //    else
                //    {
                //        Print7SegmentList (m_strl5475Buffer, 14, 0, false, true);
                //    }
                //}

                if (m_strl5471Output.Count > 0)
                {
                    //WriteOutputStringList (m_strl5471Output, EOutputTarget.OUTPUT_TracePanel, 0, true);
                    m_objTraceQueue.EnqueueTraceLines (m_strl5471Output);
                }

                //if ((m_eBootDevice == EBootDevice.BOOT_Disk_Fixed ||
                //     m_eBootDevice == EBootDevice.BOOT_Disk_Removable) &&
                //     m_bDebugDiskIPL)
                //{
                //    if (strlErrors.Count > 0)
                //    {
                //        WriteOutputStringList (strlErrors, EOutputTarget.OUTPUT_TracePanel, 0, true);
                //    }
                //}

                if (m_bDelayInstructions)
                {
                    Thread.Sleep (m_iSleepInterval);
                }
            }

            m_objPrintQueue.PrintQueuedLines ();

            SetIsInEmulator (false);
        }

        private void BreakOrContinue ()
        {
            if (!m_bAsyncRun)
            {
                return;
            }

            // <ctrl>-R   SystemReset           Any time                          PSTATE_1_NoProgramLoaded
            // F5         Run with breakpoints  Program loaded, running or not    PSTATE_3_ProgramRunningRunWithBreakPoints
            // <ctrl>-F5  FreeRun               Program loaded, running or not    PSTATE_3_ProgramRunningFreeRun
            // <shft>-F5  Stop                  Program loaded and running        PSTATE_5_Aborted
            // F7         Stop                  Program loaded and running        PSTATE_5_Aborted
            // F8         UnloadProgram         Program loaded, running or not    PSTATE_1_NoProgramLoaded
            // F10        SingleStep            Program loaded, running or not    PSTATE_3_ProgramRunningSingleStep
            if (m_objEmulatorState.IsFreeRun ())
            {
                m_objPrintQueue.PrintQueuedLines ();
                return;
            }
            else if (m_objEmulatorState.IsSingleStep ())
            {
                // Wait until one of the run mode keys is pressed, or ESC to abort
                if (m_objEmulatorState.IsSingleStep ())
                {
                    m_objEmulatorState.SetPaused ();

                    while (m_objEmulatorState.IsPaused ())
                    {
                        Thread.Sleep (250);
                        if (m_objEmulatorState.IsRunning ()    || // F10 pressed
                            m_objEmulatorState.IsFreeRun ()    ||
                            m_objEmulatorState.IsRunWithBreakPoints ())
                        {
                            return;
                        }
                        else if (m_objEmulatorState.IsProgramUnloaded () ||
                                 m_objEmulatorState.IsAborted ()) // stop
                        {
                            throw new Exception (ESCAPE_KEY);
                        }
                    }
                }
            }
            else
            {
                // Check for breakpoints
                if (m_dictBreakpoints.Count > 0 &&
                    m_dictBreakpoints.ContainsKey (m_iaIAR[m_iIL]))
                {
                    // Handle breakpoint
                    //CBreakpoint bkp = m_dictBreakpoints[m_iaIAR[m_iIL]];
                    return;
                }
            }
        }

        private void PrepKeyboardTest ()
        {
            m_eKeyboard = EKeyboard.KEY_5475;
            m_b5475_Keys_Unlocked = true;
            m_b5475_SNS_3L_Keyboard_Interrupts_Enabled = true;
        }

        private void HandleKeyboardInput ()
        {
            if (!m_bKeyboardInputEnabled ||
                (!m_bAsyncRun && !Console.KeyAvailable))
            {
                return;
            }

            if (!m_bAsyncRun)
            {
                ConsoleKeyInfo cki = Console.ReadKey (true);
                m_strKeyStroke = cki.Key.ToString ();
                m_cKeyStroke   = cki.KeyChar; // m_strKeyStroke.Length == 1 ? m_strKeyStroke[0] : '\x00';
                m_bKeyCtrl     = cki.Modifiers == ConsoleModifiers.Control;
            }

            if (m_bKeyAlt)
            {
                return;
            }

            if (m_eKeyboard == EKeyboard.KEY_5471)
            {
                if (m_iaIAR[IL_1_Keyboard] > 0)
                {
                    // Keyboard interrupt IAR loaded, so check for keystroke
                    if ((m_i5471StatusFlags & STAT_Kbd5471_Request_Key_Interrupt_Enabled) > 0)
                    {
                        if (m_strKeyStroke == "F4") // Request
                        {
                            m_i5471StatusFlags |= STAT_Kbd5471_Request_Key_Interrupt;
                            m_iIL = IL_1_Keyboard;
                        }
                    }

                    if ((m_i5471StatusFlags & STAT_Kbd5471_Other_Interrupts_Enabled) > 0)
                    {
                        if (m_strKeyStroke == "F6") // Return
                        {
                            m_i5471StatusFlags |= STAT_Kbd5471_Return_Or_Data_Key_Interrupt_Pending;
                            m_i5471StatusFlags |= STAT_Kbd5471_Return_Key;
                            m_iIL = IL_1_Keyboard;
                        }
                        else if (m_strKeyStroke == "Enter") // End
                        {
                            m_i5471StatusFlags |= STAT_Kbd5471_End_Or_Cancel_Interrupt_Pending;
                            m_i5471StatusFlags |= STAT_Kbd5471_End_Key;
                            m_iIL = IL_1_Keyboard;
                        }
                        else if (m_strKeyStroke == "Escape") // Cancel
                        {
                            m_i5471StatusFlags |= STAT_Kbd5471_End_Or_Cancel_Interrupt_Pending;
                            m_i5471StatusFlags |= STAT_Kbd5471_Cancel_Key;
                            m_iIL = IL_1_Keyboard;
                        }
                        else
                        {
                            //char cKeystroke = cki.KeyChar;
                            m_y5475Keystroke = (byte)(m_cKeyStroke & 0x00FF);
                            if (IsPrintable (m_y5475Keystroke))
                            {
                                m_y5471KeyboardInput = ConvertASCIItoEBCDIC (m_y5475Keystroke);
                                m_i5471StatusFlags |= STAT_Kbd5471_Return_Or_Data_Key_Interrupt_Pending;
                                m_i5471StatusFlags &= ~STAT_Kbd5471_Return_Key;
                                m_iIL = IL_1_Keyboard;
                            }
                        }
                    }
                }
            }
            else if (m_eKeyboard == EKeyboard.KEY_5475)
            {
                if (m_b5475_Keys_Unlocked)
                {
                    //Reset5475StatusFlags ();

                    if (m_strKeyStroke == "F1") // Read key
                    {
                        Reset5475Keys ();

                        m_b5475_SNS_2H_Read_Key_Pressed         = true;
                        m_b5475_SNS_3L_Any_Function_Key_Pressed = true;
                        m_b5475_SNS_3L_Any_Data_Key             = false;
                        m_y5475KeyboardInput                    = 0x00;
                        m_bKeyboardInputEnabled = false;
                        m_bDataKeysEnabled      = false;
                        m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_Function_Key;

                        if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled)
                        {
                            m_iIL = IL_1_Keyboard;
                        }
                    }
                    else if (m_strKeyStroke == "F2") // Skip key
                    {
                        Reset5475Keys ();

                        m_b5475_SNS_2L_Skip_Key_Pressed         = true;
                        m_b5475_SNS_3L_Any_Function_Key_Pressed = true;
                        m_b5475_SNS_3L_Any_Data_Key             = false;
                        m_y5475KeyboardInput                    = 0x00;
                        m_bKeyboardInputEnabled = false;
                        m_bDataKeysEnabled      = false;
                        m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_Function_Key;

                        if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled)
                        {
                            m_iIL = IL_1_Keyboard;
                        }
                    }
                    else if (m_strKeyStroke == "F3") // Duplicate key
                    {
                        Reset5475Keys ();

                        m_b5475_SNS_2L_Dup_Key_Pressed = true;
                        m_b5475_SNS_3L_Any_Function_Key_Pressed = true;
                        m_b5475_SNS_3L_Any_Data_Key             = false;
                        m_y5475KeyboardInput                    = 0x00;
                        m_bKeyboardInputEnabled = false;
                        m_bDataKeysEnabled      = false;
                        m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_Function_Key;

                        if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled)
                        {
                            m_iIL = IL_1_Keyboard;
                        }
                    }
                    else if (m_strKeyStroke == "F4") // Field Erase key
                    {
                        Reset5475Keys ();

                        m_b5475_SNS_2H_Field_Erase_Key_Pressed  = true;
                        m_b5475_SNS_3L_Any_Function_Key_Pressed = true;
                        m_b5475_SNS_3L_Any_Data_Key             = false;
                        m_y5475KeyboardInput                    = 0x00;
                        m_bKeyboardInputEnabled = false;
                        m_bDataKeysEnabled      = false;
                        m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_Function_Key;

                        if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled)
                        {
                            m_iIL = IL_1_Keyboard;
                        }
                    }
                    else if (m_strKeyStroke == "F5") // Record Erase switch
                    {
                        Reset5475Keys ();

                        m_b5475_SNS_2L_Record_Erase_Switch_Operated = true;
                        m_b5475_SNS_3L_Any_Function_Key_Pressed = false;
                        m_b5475_SNS_3L_Any_Data_Key             = false;
                        m_y5475KeyboardInput                    = 0x00;
                        m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_Function_Key;

                        if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled)
                        {
                            m_iIL = IL_1_Keyboard;
                        }
                    }
                    else if (m_strKeyStroke == "F6") // Release key
                    {
                        Reset5475Keys ();

                        m_b5475_SNS_2H_Release_Key_Pressed      = true;
                        m_b5475_SNS_3L_Any_Function_Key_Pressed = true;
                        m_b5475_SNS_3L_Any_Data_Key             = false;
                        m_y5475KeyboardInput                    = 0x00;
                        m_bKeyboardInputEnabled = false;
                        m_bDataKeysEnabled      = false;
                        m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_Function_Key;

                        if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled)
                        {
                            m_iIL = IL_1_Keyboard;
                        }
                    }
                    else if (m_strKeyStroke == "F7") // Error Reset key
                    {
                        Reset5475Keys ();

                        m_b5475_SNS_2H_Error_Reset_Key_Pressed  = true;
                        m_b5475_SNS_3L_Any_Function_Key_Pressed = true;
                        m_b5475_SNS_3L_Any_Data_Key             = false;
                        m_y5475KeyboardInput                    = 0x00;
                        m_bKeyboardInputEnabled = false;
                        m_bDataKeysEnabled      = false;
                        m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_Function_Key;

                        if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled)
                        {
                            m_iIL = IL_1_Keyboard;
                        }
                    }
                    else if (m_strKeyStroke == "F8") // Program One key
                    {
                        Reset5475Keys ();

                        m_b5475_SNS_2H_Program_1_Key_Pressed    = true;
                        m_b5475_SNS_2H_Program_2_Key_Pressed    = false;
                        m_b5475_SNS_3L_Any_Function_Key_Pressed = true;
                        m_b5475_SNS_3L_Any_Data_Key             = false;
                        m_b5475_Program_One_Selected            = true;
                        m_y5475KeyboardInput                    = 0x00;
                        m_bKeyboardInputEnabled = false;
                        m_bDataKeysEnabled      = false;
                        m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_Function_Key;

                        if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled &&
                            m_b5475_SNS_2L_Program_Switch_On)
                        {
                            m_iIL = IL_1_Keyboard;
                        }
                    }
                    else if (m_strKeyStroke == "F9") // Program Two key
                    {
                        Reset5475Keys ();

                        m_b5475_SNS_2H_Program_1_Key_Pressed    = false;
                        m_b5475_SNS_2H_Program_2_Key_Pressed    = true;
                        m_b5475_SNS_3L_Any_Function_Key_Pressed = true;
                        m_b5475_SNS_3L_Any_Data_Key             = false;
                        m_b5475_Program_One_Selected            = false;
                        m_y5475KeyboardInput                    = 0x00;
                        m_bKeyboardInputEnabled = false;
                        m_bDataKeysEnabled      = false;
                        m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_Function_Key;

                        if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled &&
                            m_b5475_SNS_2L_Program_Switch_On)
                        {
                            m_iIL = IL_1_Keyboard;
                        }
                    }
                    else if (m_strKeyStroke == "F10") // Toggle Program switch
                    {
                        Reset5475Keys ();

                        m_b5475_SNS_2L_Program_Switch_On        = !m_b5475_SNS_2L_Program_Switch_On;
                        m_b5475_SNS_3L_Any_Function_Key_Pressed = false;
                        m_b5475_SNS_3L_Any_Data_Key             = false;
                        m_y5475KeyboardInput                    = 0x00;
                        m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_Function_Key;

                        if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled)
                        {
                            m_iIL = IL_1_Keyboard;
                        }
                    }
                    else if (m_strKeyStroke == "F11") // Toggle Auto Record Release
                    {
                        Reset5475Keys ();

                        m_b5475_SNS_2L_Auto_Record_Release_Switch_On = !m_b5475_SNS_2L_Auto_Record_Release_Switch_On;
                        m_b5475_SNS_3L_Any_Function_Key_Pressed      = false;
                        m_b5475_SNS_3L_Any_Data_Key                  = false;
                        m_y5475KeyboardInput                         = 0x00;
                        m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_Function_Key;

                        if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled)
                        {
                            m_iIL = IL_1_Keyboard;
                        }
                    }
                    else if (m_strKeyStroke == "F12") // End multi-punch
                    {
                        Reset5475Keys ();

                        if (m_yMultiPunchCharacter > 0)
                        {
                            m_y5475KeyboardInput   = m_yaEbcdicPunchTable[(m_yMultiPunchCharacter & 0x3F)];
                            m_yMultiPunchCharacter = 0;
                            m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_Multi_Punch;
                            m_b5475_SNS_3L_Any_Function_Key_Pressed = false;
                            m_b5475_SNS_3L_Any_Data_Key             = true;
                            m_b5475_Data_Key_Latched                = true;
                            m_bKeyboardInputEnabled = false;
                            m_bDataKeysEnabled      = false;

                            if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled)
                            {
                                m_iIL = IL_1_Keyboard;
                            }
                        }
                    }
                    else if (m_bDataKeysEnabled)
                    {
                        //char cKeystroke = cki.KeyChar;
                        m_y5475Keystroke = (byte)(m_cKeyStroke & 0x00FF);

                        // Multi-punch characters
                        //if (cki.Modifiers == ConsoleModifiers.Control)
                        if (m_bKeyCtrl)
                        {
                            if (m_cKeyStroke == '0')
                            {
                                m_yMultiPunchCharacter |= 0x10;
                            }
                            else if (m_cKeyStroke == '1')
                            {
                                m_yMultiPunchCharacter |= 0x01;
                            }
                            else if (m_cKeyStroke == '2')
                            {
                                m_yMultiPunchCharacter |= 0x02;
                            }
                            else if (m_cKeyStroke == '3')
                            {
                                m_yMultiPunchCharacter |= 0x03;
                            }
                            else if (m_cKeyStroke == '4')
                            {
                                m_yMultiPunchCharacter |= 0x04;
                            }
                            else if (m_cKeyStroke == '5')
                            {
                                m_yMultiPunchCharacter |= 0x05;
                            }
                            else if (m_cKeyStroke == '6')
                            {
                                m_yMultiPunchCharacter |= 0x06;
                            }
                            else if (m_cKeyStroke == '7')
                            {
                                m_yMultiPunchCharacter |= 0x07;
                            }
                            else if (m_cKeyStroke == '8')
                            {
                                m_yMultiPunchCharacter |= 0x08;
                            }
                            else if (m_cKeyStroke == '9')
                            {
                                m_yMultiPunchCharacter |= 0x09;
                            }
                            else if (m_cKeyStroke == '-')
                            {
                                m_yMultiPunchCharacter |= 0x20;
                            }

                            m_e5475Interrupt     = E5475Interrupt.E5475_Interrupt_None;
                            m_y5475KeyboardInput = 0x00;
                            m_bMultiPunch        = true;

                            Show5475Status ();
                            return; // No action taken until F12, end of multi-punch
                        }
                        else
                        {
                            m_bMultiPunch = false;
                        }

                        if (m_strKeyStroke == "Escape") // Escape character
                        //if (cki.KeyChar == 0x001b) // Escape character
                        {
                            Reset5475Keys ();

                            m_e5475Interrupt       = E5475Interrupt.E5475_Interrupt_None;
                            m_yMultiPunchCharacter = 0;
                            m_y5475KeyboardInput   = 0x00;
                            Show5475Status ();
                            return; // Clear multi-punch buffer and get out
                        }

                        if (IsPrintable (m_y5475Keystroke))
                        {
                            Reset5475Keys ();

                            if (m_b5475_SNS_2L_Program_Switch_On)
                            {
                                // Keyboard is in upper shift
                            }
                            else
                            {
                                // Keyboard is in lower shift
                            }

                            //m_y5475KeyboardInput = ConvertASCIItoEBCDIC (ToUpper (m_y5475Keystroke));
                            m_y5475KeyboardInput = ConvertASCIItoEBCDIC (m_y5475Keystroke);
                            if (m_b5475_Keyboard_Program_Numeric_Mode)
                            {
                                m_b5475_SNS_1L_Invalid_Character_Detected = (m_y5475Keystroke < '0' || m_y5475Keystroke > '9');
                                if (m_b5475_SNS_1L_Invalid_Character_Detected)
                                {
                                    //if (m_y5475KeyboardInput >= 0x81 && // 'a'
                                    //    m_y5471KeyboardInput <= 0x86)   // 'f'
                                    //{
                                    //    // Convert to upper case
                                    //    m_y5471KeyboardInput += 0x40;
                                    //}
                                    // Convert to expected invalid characters
                                    // C1 C2 C3 C4 C5 C6 <ABCDEF>
                                    // 4C 4D 7D D0 5A 4F <<('}]!>
                                    if (m_y5475KeyboardInput == 0xC1 || // 'A'
                                        m_y5475KeyboardInput == 0x81)   // 'a'
                                    {
                                        m_y5475KeyboardInput = 0x4C;
                                    }
                                    else if (m_y5475KeyboardInput == 0xC2 || // 'B'
                                             m_y5475KeyboardInput == 0x82)   // 'b'
                                    {
                                        m_y5475KeyboardInput = 0x4D;
                                    }
                                    else if (m_y5475KeyboardInput == 0xC3 || // 'C'
                                             m_y5475KeyboardInput == 0x83)   // 'c'
                                    {
                                        m_y5475KeyboardInput = 0x7D;
                                    }
                                    else if (m_y5475KeyboardInput == 0xC4 || // 'D'
                                             m_y5475KeyboardInput == 0x84)   // 'd'
                                    {
                                        m_y5475KeyboardInput = 0xD0;
                                    }
                                    else if (m_y5475KeyboardInput == 0xC5 || // 'E'
                                             m_y5475KeyboardInput == 0x85)   // 'e'
                                    {
                                        m_y5475KeyboardInput = 0x5A;
                                    }
                                    else if (m_y5475KeyboardInput == 0xC6 || // 'F'
                                             m_y5475KeyboardInput == 0x86)   // 'f'
                                    {
                                        m_y5475KeyboardInput = 0x4F;
                                    }
                                }
                            }

                            m_b5475_SNS_3L_Any_Data_Key             = true;
                            m_b5475_SNS_3L_Any_Function_Key_Pressed = false;
                            m_b5475_Data_Key_Latched                = true;

                            if (IsLower (m_cKeyStroke))
                            {
                                m_b5475_SNS_1L_Lower_Shift_Key_Pressed = true;
                            }

                            m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_Data_Key;
                            m_bKeyboardInputEnabled = false;
                            m_bDataKeysEnabled      = false;

                            if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled)
                            {
                                m_iIL = IL_1_Keyboard;
                            }
                        }
                    }
                }

                Show5475Status ();
            }
        }

        static string FormatTotalCPUTime (double dCPUTime, bool bShowInput = true)
        {
            StringBuilder sbFormattedTime = new StringBuilder ();

            if (bShowInput)
            {
                sbFormattedTime.Append (string.Format ("{0}  ", dCPUTime));
            }

            dCPUTime /= 100; // Convert to microseconds from fractional microseconds

            if (dCPUTime > 999999)
            {
                dCPUTime /= 1000000; // convert to seconds
                if (dCPUTime > 300)
                {
                    int iMinutes = (int)dCPUTime / 60;
                    double dSeconds = dCPUTime % 60;
                    sbFormattedTime.Append (string.Format ("{0:####}:{1:00.###}", iMinutes, dSeconds));
                }
                else
                {
                    sbFormattedTime.Append (string.Format ("{0:#####.###}s", dCPUTime));
                }
            }
            else if (dCPUTime > 999)
            {
                dCPUTime /= 1000; // convert to milliseconds
                sbFormattedTime.Append (string.Format ("{0:#####.###}ms", dCPUTime));
            }
            else // keep as microseconds
            {
                sbFormattedTime.Append (string.Format ("{0:#####.00}{1}s", dCPUTime, MICRO_SIGN));
            }

            return sbFormattedTime.ToString ();
        }

        //protected string FormatTotalCPUTime (double dCPUTime)
        //{
        //    StringBuilder sbFormattedTime = new StringBuilder ();

        //    if (dCPUTime > 999999)
        //    {
        //        dCPUTime /= 1000000; // convert to seconds
        //        if (dCPUTime > 300)
        //        {
        //            int iMinutes = (int)dCPUTime / 60;
        //            int iSeconds = (int)dCPUTime % 60;
        //            sbFormattedTime.Append (string.Format ("{0:####}:{1:00}", iMinutes, iSeconds));
        //        }
        //        else
        //        {
        //            sbFormattedTime.Append (string.Format ("{0:#####}s", dCPUTime));
        //        }
        //    }
        //    else if (dCPUTime > 999)
        //    {
        //        dCPUTime /= 1000; // convert to milliseconds
        //        sbFormattedTime.Append (string.Format ("{0:#####}ms", dCPUTime));
        //    }
        //    else // keep as microseconds
        //    {
        //        sbFormattedTime.Append (string.Format ("{0:#####.00}{1}s", dCPUTime, MICRO_SIGN));
        //    }

        //    return sbFormattedTime.ToString ();
        //}

        protected string FormatTotalTime (ulong ulTotalIime)
        {
            // hh:mm:ss.mmm yyy nn
            // sssmmmyyynn
            // nn = ulTotalTime % 100;
            // yyy = (ulTotalTime / 100) % 1000;
            // mmm = (ulTotalTime / 100000) % 1000;
            StringBuilder strbTotalTime = new StringBuilder ();

            ulong ulNanoSeconds  = ulTotalIime % 100;
            ulong ulMicroSeconds = ulTotalIime / 100;
            ulong ulMicroSecOnly = ulMicroSeconds % 1000;
            ulong ulMilliSeconds = ulMicroSeconds / 1000;
            ulong ulMilliSecOnly = ulMilliSeconds % 1000;
            ulong ulSeconds      = ulMilliSeconds / 1000;
            ulong ulSecOnly      = ulSeconds % 60;
            ulong ulMinutes      = ulSeconds / 60;
            ulong ulMinOnly      = ulMinutes % 60;
            ulong ulHours        = ulMinutes / 60;

            if (ulHours > 0)
            {
                strbTotalTime.Append (ulHours.ToString () + ':');
            }

            if (ulMinutes > 0 ||
                ulHours   > 0)
            {
                string strMinutes = ulMinOnly.ToString ();
                string strPadding = new string (ulHours > 0 ? '0' : ' ', 2 - strMinutes.Length);
                strbTotalTime.Append ((strbTotalTime.Length > 0 ? strPadding : "") + strMinutes + ':');
            }

            if (ulSeconds > 0 ||
                ulMinutes > 0 ||
                ulHours   > 0)
            {
                string strSeconds = ulSecOnly.ToString ();
                string strPadding = new string (ulMinutes > 0 ? '0' : ' ', 2 - strSeconds.Length);
                strbTotalTime.Append ((strbTotalTime.Length > 0 ? strPadding : "") + strSeconds + '.');
            }

            if (ulMilliSeconds > 0)
            {
                string strMilliSeconds = ulMilliSecOnly.ToString ();
                string strPadding = new string (ulSeconds > 0 ? '0' : ' ', 3 - strMilliSeconds.Length);
                strbTotalTime.Append ((strbTotalTime.Length > 0 ? strPadding : "") + strMilliSeconds + ' ');
            }

            if (ulMicroSeconds > 0)
            {
                string strMicroSeconds = ulMicroSecOnly.ToString ();
                string strPadding = new string (ulMilliSeconds > 0 ? '0' : ' ', 3 - strMicroSeconds.Length);
                strbTotalTime.Append ((strbTotalTime.Length > 0 ? strPadding : "") + strMicroSeconds + ' ');
            }

            string strNanoSeconds = ulNanoSeconds.ToString ();
            string strPaddingFuck = new string (ulMicroSeconds > 0 ? '0' : ' ', 2 - strNanoSeconds.Length);
            strbTotalTime.Append (strPaddingFuck + strNanoSeconds);

            return strbTotalTime.ToString ();
        }

        protected void ExecuteInstruction (byte[] yaInstruction)
        {
            //    | x0   x1   x2   x3   x4   x5   x6   x7   x8   x9   xA   xB   xC   xD   xE   xF
            // ---+-------------------------------------------------------------------------------
            // 0x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
            // 1x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
            // 2x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
            // 3x | SNS  LIO   -    -    ST   L    A   -    TBN  TBF  SBN  SBF  MVI  CLI   -    -
            //    |
            // 4x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
            // 5x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
            // 6x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
            // 7x | SNS  LIO   -    -    ST   L    A   -    TBN  TBF  SBN  SBF  MVI  CLI   -    -
            //    |
            // 8x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
            // 9x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
            // Ax |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
            // Bx | SNS  LIO   -    -    ST   L    A   -    TBN  TBF  SBN  SBF  MVI  CLI   -    -
            //    |
            // Cx |  BC  TIO   LA   -    -    -    -   -     -    -    -    -    -    -    -    -
            // Dx |  BC  TIO   LA   -    -    -    -   -     -    -    -    -    -    -    -    -
            // Ex |  BC  TIO   LA   -    -    -    -   -     -    -    -    -    -    -    -    -
            // Fx | HPL  APL   JC  SIO   -    -    -   -     -    -    -    -    -    -    -    -
            switch (yaInstruction[0])
            {
                // ZAZ
                case 0x04:
                case 0x14:
                case 0x24:
                case 0x44:
                case 0x54:
                case 0x64:
                case 0x84:
                case 0x94:
                case 0xA4:
                {
                    ExecuteZAZ (yaInstruction);
                    break;
                }

                // AZ
                case 0x06:
                case 0x16:
                case 0x26:
                case 0x46:
                case 0x56:
                case 0x66:
                case 0x86:
                case 0x96:
                case 0xA6:
                {
                    ExecuteAZ (yaInstruction);
                    break;
                }

                // SZ
                case 0x07:
                case 0x17:
                case 0x27:
                case 0x47:
                case 0x57:
                case 0x67:
                case 0x87:
                case 0x97:
                case 0xA7:
                {
                    ExecuteSZ (yaInstruction);
                    break;
                }

                // MVX
                case 0x08:
                case 0x18:
                case 0x28:
                case 0x48:
                case 0x58:
                case 0x68:
                case 0x88:
                case 0x98:
                case 0xA8:
                {
                    ExecuteMVX (yaInstruction);
                    break;
                }

                // ED
                case 0x0A:
                case 0x1A:
                case 0x2A:
                case 0x4A:
                case 0x5A:
                case 0x6A:
                case 0x8A:
                case 0x9A:
                case 0xAA:
                {
                    ExecuteED (yaInstruction);
                    break;
                }

                // ITC
                case 0x0B:
                case 0x1B:
                case 0x2B:
                case 0x4B:
                case 0x5B:
                case 0x6B:
                case 0x8B:
                case 0x9B:
                case 0xAB:
                {
                    ExecuteITC (yaInstruction);
                    break;
                }

                // MVC
                case 0x0C:
                case 0x1C:
                case 0x2C:
                case 0x4C:
                case 0x5C:
                case 0x6C:
                case 0x8C:
                case 0x9C:
                case 0xAC:
                {
                    ExecuteMVC (yaInstruction);
                    break;
                }

                // CLC
                case 0x0D:
                case 0x1D:
                case 0x2D:
                case 0x4D:
                case 0x5D:
                case 0x6D:
                case 0x8D:
                case 0x9D:
                case 0xAD:
                {
                    ExecuteCLC (yaInstruction);
                    break;
                }

                // ALC
                case 0x0E:
                case 0x1E:
                case 0x2E:
                case 0x4E:
                case 0x5E:
                case 0x6E:
                case 0x8E:
                case 0x9E:
                case 0xAE:
                {
                    ExecuteALC (yaInstruction);
                    break;
                }

                // SLC
                case 0x0F:
                case 0x1F:
                case 0x2F:
                case 0x4F:
                case 0x5F:
                case 0x6F:
                case 0x8F:
                case 0x9F:
                case 0xAF:
                {
                    ExecuteSLC (yaInstruction);
                    break;
                }

                // SNS   
                case 0x30:
                case 0x70:
                case 0xB0:
                {
                    ExecuteSNS (yaInstruction);
                    break;
                }

                // LIO
                case 0x31:
                case 0x71:
                case 0xB1:
                {
                    ExecuteLIO (yaInstruction);
                    break;
                }

                // ST
                case 0x34:
                case 0x74:
                case 0xB4:
                {
                    ExecuteST (yaInstruction);
                    break;
                }

                // L
                case 0x35:
                case 0x75:
                case 0xB5:
                {
                    ExecuteL (yaInstruction);
                    break;
                }

                // A
                case 0x36:
                case 0x76:
                case 0xB6:
                {
                    ExecuteA (yaInstruction);
                    break;
                }

                // TBN
                case 0x38:
                case 0x78:
                case 0xB8:
                {
                    ExecuteTBN (yaInstruction);
                    break;
                }

                // TBF
                case 0x39:
                case 0x79:
                case 0xB9:
                {
                    ExecuteTBF (yaInstruction);
                    break;
                }

                // SBN
                case 0x3A:
                case 0x7A:
                case 0xBA:
                {
                    ExecuteSBN (yaInstruction);
                    break;
                }

                // SBF
                case 0x3B:
                case 0x7B:
                case 0xBB:
                {
                    ExecuteSBF (yaInstruction);
                    break;
                }

                // MVI
                case 0x3C:
                case 0x7C:
                case 0xBC:
                {
                    ExecuteMVI (yaInstruction);
                    break;
                }

                // CLI
                case 0x3D:
                case 0x7D:
                case 0xBD:
                {
                    ExecuteCLI (yaInstruction);
                    break;
                }

                // BC
                case 0xC0:
                case 0xD0:
                case 0xE0:
                {
                    ExecuteBC (yaInstruction);
                    break;
                }

                // TIO
                case 0xC1:
                case 0xD1:
                case 0xE1:
                {
                    ExecuteTIO (yaInstruction);
                    break;
                }

                // LA
                case 0xC2:
                case 0xD2:
                case 0xE2:
                {
                    ExecuteLA (yaInstruction);
                    break;
                }

                // HPL
                case 0xF0:
                {
                    ExecuteHPL (yaInstruction);
                    break;
                }

                // APL
                case 0xF1:
                {
                    ExecuteAPL (yaInstruction);
                    break;
                }

                // JC
                case 0xF2:
                {
                    ExecuteJC (yaInstruction);
                    break;
                }

                // SIO
                case 0xF3:
                {
                    ExecuteSIO (yaInstruction);
                    break;
                }

                // data
                default:
                {
                    InvalidOpCode ();
                    break;
                }
            }

            if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[m_iIL], m_iIL)); }
        }

        #region Instruction Methods
        // ZAZ - Zero and Add Zoned
        private void ExecuteZAZ (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];
            int iOperandTwoLength = (yQByte & 0x0F) + 1,
                iOperandOneLength = (yQByte >> 4) + iOperandTwoLength;
            bool bZeroValue = true,
                 bNegative  = false;

            if (ZonedAddressesValid (objTOA, yQByte))
            {
                int iOperandOneAddress = objTOA.OperandOneAddress,
                    iOperandTwoAddress = objTOA.OperandTwoAddress;

                // Check for invalid operand overlap
                if (iOperandOneAddress < iOperandTwoAddress &&
                    iOperandOneAddress > iOperandTwoAddress - iOperandTwoLength)
                {
                    InvalidQByte ();
                    return;
                }

                // Copy the data, setting the zones for each character, zeroing all leading positiona
                while ((iOperandOneLength) > 0)
                {
                    if (iOperandTwoLength > 0)
                    {
                        m_yaMainMemory[iOperandOneAddress] = m_yaMainMemory[iOperandTwoAddress];
                        m_yaMainMemory[iOperandOneAddress] |= (byte)0xF0;
                        if ((m_yaMainMemory[iOperandOneAddress] - (byte)0xF0) > 0)
                        {
                            bZeroValue = false;
                        }
                    }
                    else
                    {
                        m_yaMainMemory[iOperandOneAddress] = (byte)0xF0;
                    }

                    iOperandOneLength--;
                    iOperandTwoLength--;
                    iOperandOneAddress--;
                    iOperandTwoAddress--;
                }

                // Now set the sign bit in the rightmost position
                if ((m_yaMainMemory[objTOA.OperandTwoAddress] & 0x0F) == 0x00)
                {
                    m_yaMainMemory[objTOA.OperandOneAddress] = 0xF0;
                }
                else if ((m_yaMainMemory[objTOA.OperandTwoAddress] & 0xF0) != 0xF0)
                {
                    m_yaMainMemory[objTOA.OperandOneAddress] &= 0xDF;
                    m_yaMainMemory[objTOA.OperandOneAddress] |= 0xD0;
                    bNegative = true;
                }

                m_iaARR[m_iIL] = objTOA.OperandOneAddress;
                if (m_objEmulatorState.IsSingleStep () && OnNewARR != null) { OnNewARR (this, new NewARREventArgs (m_iaARR[m_iIL], m_iIL)); }
            }

            if (bZeroValue)
            {
                m_aCR[m_iIL].SetEqual ();
                if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
            }
            else if (bNegative)
            {
                m_aCR[m_iIL].SetLow ();
                if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
            }
            else
            {
                m_aCR[m_iIL].SetHigh ();
                if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
            }
        }

        // AZ - Add Zoned Decimal
        private void ExecuteAZ (byte[] yaInstruction)
        {
            ExecuteDecimalArithmetic (yaInstruction, true);
        }

        // SZ - Subtract Zoned Decimal
        private void ExecuteSZ (byte[] yaInstruction)
        {
            ExecuteDecimalArithmetic (yaInstruction, false);
        }

        // ALC - Add Logical Characters
        private void ExecuteALC (byte[] yaInstruction)
        {
            m_aCR[m_iIL].ResetBinaryOverflow ();
            if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }

            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = (byte)(yaInstruction[1] + 1);

            if (BinaryAddressesValid (objTOA, yQByte))
            {
                bool bCarry = false,
                     bZero  = true;

                int iOperandOneAddress = objTOA.OperandOneAddress,
                    iOperandTwoAddress = objTOA.OperandTwoAddress;

                while (yQByte-- > 0)
                {
                    int iOperand1 = m_yaMainMemory[iOperandOneAddress],
                        iOperand2 = m_yaMainMemory[iOperandTwoAddress],
                        iResult = iOperand1 + iOperand2;

                    if (bCarry)
                    {
                        iResult++;
                        bCarry = false;
                    }

                    if ((iResult & 0xFF) > 0x00)
                    {
                        bZero = false;
                    }

                    if (iResult > 0xFF)
                    {
                        iResult -= 0x0100;
                        bCarry = true;
                    }

                    m_yaMainMemory[iOperandOneAddress] = (byte)(iResult & 0xFF);
                    iOperandOneAddress--;
                    iOperandTwoAddress--;
                }

                // Update the condition register with the result
                if (bCarry)
                {
                    m_aCR[m_iIL].SetBinaryOverflow ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }

                if (bZero)
                {
                    m_aCR[m_iIL].SetEqual ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
                else if (bCarry)
                {
                    // Carry occured out of high-order byte and result is non-zero
                    m_aCR[m_iIL].SetHigh ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
                else
                {
                    // NO carry occured out of high-order byte and result is non-zero
                    m_aCR[m_iIL].SetLow ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
            }
        }

        // SLC - Subtract Logical Characters
        private void ExecuteSLC (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = (byte)(yaInstruction[1] + 1);

            if (BinaryAddressesValid (objTOA, yQByte))
            {
                bool bBorrow = false,
                     bZero   = true;

                int iOperandOneAddress = objTOA.OperandOneAddress;
                int iOperandTwoAddress = objTOA.OperandTwoAddress;

                while (yQByte-- > 0)
                {
                    int iOperand1 = m_yaMainMemory[iOperandOneAddress],
                        iOperand2 = m_yaMainMemory[iOperandTwoAddress],
                        iResult   = iOperand1 - iOperand2;

                    if (bBorrow)
                    {
                        iResult--;
                        bBorrow = false;
                    }

                    if ((iResult & 0xFF) > 0x00)
                    {
                        bZero = false;
                    }

                    if ((iResult - (iResult & 0xFF)) != 0)
                    {
                        bBorrow = true;
                    }

                    m_yaMainMemory[iOperandOneAddress] = (byte)(iResult & 0xFF);
                    iOperandOneAddress--;
                    iOperandTwoAddress--;
                }

                // Update the condition register with the result
                if (bZero)
                {
                    m_aCR[m_iIL].SetEqual ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
                else if (bBorrow)
                {
                    // Borrow occured out of high-order byte and result is non-zero
                    m_aCR[m_iIL].SetLow ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
                else
                {
                    // NO borrow occured out of high-order byte and result is non-zero
                    m_aCR[m_iIL].SetHigh ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
            }
        }

        // CLC - Compare Logical Characters
        private void ExecuteCLC (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];

            if (BinaryAddressesValid (objTOA, yQByte))
            {
                int iOperandOneAddress = objTOA.OperandOneAddress - yQByte;
                int iOperandTwoAddress = objTOA.OperandTwoAddress - yQByte;

                // Shortcut: if both addresses match, both operands can't differ
                if (iOperandOneAddress == iOperandTwoAddress)
                {
                    m_aCR[m_iIL].SetEqual ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                    return;
                }

                int iOffset = (int)yQByte + 1;

                while (iOffset-- > 0)
                {
                    if (m_yaMainMemory[iOperandOneAddress] < m_yaMainMemory[iOperandTwoAddress])
                    {
                        m_aCR[m_iIL].SetLow ();
                        if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                        return;
                    }
                    else if (m_yaMainMemory[iOperandOneAddress] > m_yaMainMemory[iOperandTwoAddress])
                    {
                        m_aCR[m_iIL].SetHigh ();
                        if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                        return;
                    }
                    else
                    {
                        iOperandOneAddress++;
                        iOperandTwoAddress++;
                    }
                }

                m_aCR[m_iIL].SetEqual ();
                if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
            }
        }

        // MVC - Move Characters
        private void ExecuteMVC (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];

            if (BinaryAddressesValid (objTOA, yQByte))
            {
                int iOperandOneAddress = objTOA.OperandOneAddress;
                int iOperandTwoAddress = objTOA.OperandTwoAddress;
                int iOffset            = yQByte + 1;

                if (m_eUIRunMode == EUIRunMode.RUN_LoadFromIPL &&
                    m_iIPLCardCount > 1)
                {
                    int iSourceBeginAddress   = iOperandTwoAddress - yQByte;
                    int iTargetBeginAddress   = iOperandOneAddress - yQByte;

                    if (iSourceBeginAddress   >= m_iOldStartDASMAddress &&
                        iOperandTwoAddress    <= m_iOldEndDASMAddress   &&
                        !(iTargetBeginAddress >= m_iOldStartDASMAddress &&
                          iOperandOneAddress  <= m_iOldEndDASMAddress))
                    {
                        if (m_iNewStartDASMAddress == 0x0000)
                        {
                            m_iNewStartDASMAddress = iTargetBeginAddress;
                        }
                        else
                        {
                            m_iNewStartDASMAddress = Math.Min (m_iNewStartDASMAddress, iTargetBeginAddress);
                        }
                        m_iNewEndDASMAddress       = Math.Max (m_iNewEndDASMAddress,   iOperandOneAddress);
                    }
                    //m_iOldStartDASMAddress   = 0x0000;
                    //m_iOldEndDASMAddress     = CARD_IMAGE_IPL_SIZE;
                    //m_iOldDASMEntryPoint     = 0x0000;
                    //m_iNewStartDASMAddress   = 0x0000;
                    //m_iNewEndDASMAddress     = 0x0000;
                    //m_iNewDASMEntryPoint     = 0x0000;
                }

                if (m_eUIRunMode == EUIRunMode.RUN_LoadFromIPL &&
                    m_iIPLCardCount    > 1                     &&
                    iOperandOneAddress > CARD_IMAGE_IPL_SIZE)
                {
                    m_iProgramSizeIPL += iOffset;
                }

                while (iOffset-- > 0)
                {
                    m_yaMainMemory[iOperandOneAddress--] = m_yaMainMemory[iOperandTwoAddress--];
                }
            }
        }

        // MVX - Move Hex Character
        private void ExecuteMVX (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            if (!IsAddressValid (objTOA.OperandOneAddress) ||
                !IsAddressValid (objTOA.OperandTwoAddress))
            {
                return;
            }

            byte yQByte = yaInstruction[1];
            byte yOperand1 = m_yaMainMemory[objTOA.OperandOneAddress];
            byte yOperand2 = m_yaMainMemory[objTOA.OperandTwoAddress];

            if (yQByte == 0x00) // Zone to Zone
            {
                yOperand1 &= 0x0F;
                yOperand1 |= (byte)(yOperand2 & 0xF0);
            }
            else if (yQByte == 0x01) // Numeric to Zone
            {
                yOperand1 &= 0x0F;
                yOperand2 <<= 4;
                yOperand1 |= yOperand2;
            }
            else if (yQByte == 0x02) // Zone to Numeric
            {
                yOperand1 &= 0xF0;
                yOperand2 >>= 4;
                yOperand1 |= yOperand2;
            }
            else if (yQByte == 0x03) // Numeric to Numeric
            {
                yOperand1 &= 0xF0;
                yOperand1 |= (byte)(yOperand2 & 0x0F);
            }

            m_yaMainMemory[objTOA.OperandOneAddress] = yOperand1;
        }

        // ED - Edit
        private void ExecuteED (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];

            if (BinaryAddressesValid (objTOA, yQByte))
            {
                int iOperandOneLength  = (int)yQByte + 1,
                    iOperandOneAddress = objTOA.OperandOneAddress,
                    iOperandTwoAddress = objTOA.OperandTwoAddress,
                    iOperandOneStart   = iOperandOneAddress - iOperandOneLength,
                    iOperandTwoStart   = iOperandTwoAddress - iOperandOneLength;

                // Operands may not overlap
                if ((iOperandOneStart   >= iOperandTwoStart && iOperandOneStart   <= iOperandTwoAddress) ||
                    (iOperandOneAddress >= iOperandTwoStart && iOperandOneAddress <= iOperandTwoAddress) ||
                    (iOperandTwoStart   >= iOperandOneStart && iOperandTwoStart   <= iOperandOneAddress) ||
                    (iOperandTwoAddress >= iOperandOneStart && iOperandTwoAddress <= iOperandOneAddress))
                {
                    InvalidQByte ();
                    return;
                }

                bool bZero = true;
                bool bNegative = (m_yaMainMemory[iOperandTwoAddress] & 0xF0) != 0xF0;
                int iOffsetTwo = 0;

                for (int iOffsetOne = 0; iOffsetOne < iOperandOneLength; iOffsetOne++)
                {
                    if (m_yaMainMemory[iOperandOneAddress - iOffsetOne] == 0x20)
                    {
                        m_yaMainMemory[iOperandOneAddress - iOffsetOne] = m_yaMainMemory[iOperandTwoAddress - iOffsetTwo];
                        iOffsetTwo++;

                        m_yaMainMemory[iOperandOneAddress - iOffsetOne] |= 0xF0;

                        if ((m_yaMainMemory[iOperandOneAddress - iOffsetOne] & 0x0F) > 0)
                        {
                             bZero = false;
                        }
                    }
                }

                // Set condition register
                if (bZero)
                {
                    m_aCR[m_iIL].SetEqual ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
                else if (bNegative)
                {
                    m_aCR[m_iIL].SetLow ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
                else
                {
                    m_aCR[m_iIL].SetHigh ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
            }
        }

        // ITC - Insert and Test Characters
        private void ExecuteITC (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];

            int iOperandOneLength  = (int)yQByte + 1,
                iOperandOneAddress = objTOA.OperandOneAddress,
                iOperandTwoAddress = objTOA.OperandTwoAddress;

            // NOTE: Operand One addresses leftmost byte instead of rightmost
            if ((iOperandOneAddress + iOperandOneLength > m_yaMainMemory.Length) ||
                (iOperandTwoAddress > m_yaMainMemory.Length))
            {
                InvalidAddress ();
                return;
            }

            //iOperandOneLength++;
            m_iaARR[m_iIL] = iOperandOneAddress + iOperandOneLength; // Default ARR value
            if (m_objEmulatorState.IsSingleStep () && OnNewARR != null) { OnNewARR (this, new NewARREventArgs (m_iaARR[m_iIL], m_iIL)); }

            for (int iIdx = 0; iIdx < iOperandOneLength; iIdx++)
            {
                // Upon finding the first significant numeric digit, job is finished
                if (m_yaMainMemory[iOperandOneAddress + iIdx] >= 0xF1 &&
                    m_yaMainMemory[iOperandOneAddress + iIdx] <= 0xF9)
                {
                    m_iaARR[m_iIL] = iOperandOneAddress + iIdx;
                    if (m_objEmulatorState.IsSingleStep () && OnNewARR != null) { OnNewARR (this, new NewARREventArgs (m_iaARR[m_iIL], m_iIL)); }
                    break;
                }

                m_yaMainMemory[iOperandOneAddress + iIdx] = m_yaMainMemory[iOperandTwoAddress];
            }

            // No condition register changes
        }

        // MVI - Move Logical Immediate
        private void ExecuteMVI (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }

            m_yaMainMemory[iOperandAddress] = yaInstruction[1];
        }

        // CLI - Compare Logical Immediate
        private void ExecuteCLI (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }

            byte yQByte = yaInstruction[1];

            if (m_yaMainMemory[iOperandAddress] < yQByte)
            {
                m_aCR[m_iIL].SetLow ();
                if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
            }
            else if (m_yaMainMemory[iOperandAddress] > yQByte)
            {
                m_aCR[m_iIL].SetHigh ();
                if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
            }
            else
            {
                m_aCR[m_iIL].SetEqual ();
                if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
            }
        }

        // SBN - Set Bits On Masked
        private void ExecuteSBN (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }


            m_yaMainMemory[iOperandAddress] |= yaInstruction[1];
        }

        // SBF - Set Bits Off Masked
        private void ExecuteSBF (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }


            m_yaMainMemory[iOperandAddress] &= (byte)~yaInstruction[1];
        }

        // TBN - Test Bits On Masked
        private void ExecuteTBN (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }

            byte yQByte = yaInstruction[1];

            if ((yQByte & m_yaMainMemory[iOperandAddress]) != yQByte)
            {
                m_aCR[m_iIL].SetTestFalse ();
                if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
            }
        }

        // TBF - Test Bits Off Masked
        private void ExecuteTBF (byte[] yaInstruction)
        {
            int  iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }

            byte yQByte = yaInstruction[1];

            if ((yQByte & (byte)~m_yaMainMemory[iOperandAddress]) != yQByte)
            {
                m_aCR[m_iIL].SetTestFalse ();
                if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
            }
        }

        // A - Add to Register
        private void ExecuteA (byte[] yaInstruction)
        {
            int    iOperandAddress = GetOneOperandAddress (yaInstruction);
            byte   yQByte          = yaInstruction[1];
            int    iRegValue       = 0;
            int    iOldValue       = 0;
            string strRegisterName = "";

            if (AddressWraps (iOperandAddress, 2))
            {
                AddressWraparound ();
                return;
            }

            if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL) == (byte)ERegisterMask.REGMSK_IAR_IL)
            {
                if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL1) == (byte)ERegisterMask.REGMSK_IAR_IL1)
                {
                    iRegValue       = m_iaIAR[2];
                    strRegisterName = "IL 1 IAR";
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL2) == (byte)ERegisterMask.REGMSK_IAR_IL2)
                {
                    iRegValue       = m_iaIAR[3];
                    strRegisterName = "IL 2 IAR";
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL3) == (byte)ERegisterMask.REGMSK_IAR_IL3)
                {
                    iRegValue       = m_iaIAR[4];
                    strRegisterName = "IL 3 IAR";
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL4) == (byte)ERegisterMask.REGMSK_IAR_IL4)
                {
                    iRegValue       = m_iaIAR[5];
                    strRegisterName = "IL 4 IAR";
                }
                else
                {
                    iRegValue       = m_iaIAR[1];
                    strRegisterName = "IL 0 IAR";
                }
            }
            else
            {
                // Extract the register value
                if ((yQByte & (byte)ERegisterMask.REGMSK_PL2IAR) == (byte)ERegisterMask.REGMSK_PL2IAR)
                {
                    iRegValue       = m_iIARpl2;
                    strRegisterName = "PL 2 IAR";
                    return;
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_PL1IAR) == (byte)ERegisterMask.REGMSK_PL1IAR)
                {
                    iRegValue       = m_iIARpl1;
                    strRegisterName = "PL 1 IAR";
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR) == (byte)ERegisterMask.REGMSK_IAR)
                {
                    iRegValue       = m_iaIAR[m_iIL];
                    strRegisterName = "IAR";
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_ARR) == (byte)ERegisterMask.REGMSK_ARR)
                {
                    iRegValue       = m_iaARR[m_iIL];
                    strRegisterName = "ARR";
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_PSR) == (byte)ERegisterMask.REGMSK_PSR)
                {
                    iRegValue       = m_aCR[m_iIL].Store ();
                    strRegisterName = string.Format ("IL {0} CR", m_iIL);
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_XR2) == (byte)ERegisterMask.REGMSK_XR2)
                {
                    iRegValue       = m_iXR2;
                    strRegisterName = "XR2";
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_XR1) == (byte)ERegisterMask.REGMSK_XR1)
                {
                    iRegValue       = m_iXR1;
                    strRegisterName = "XR1";
                }
            }

            // Perform arithmetic and update CR flags
            iOldValue = iRegValue;
            iRegValue += LoadInt (iOperandAddress);
            if ((iRegValue & 0xFFFF) == 0x0000)
            {
                m_aCR[m_iIL].SetEqual ();
                if (iRegValue > 0xFFFF)
                {
                    m_aCR[m_iIL].SetBinaryOverflow ();
                }
                if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
            }
            else if (iRegValue > 0xFFFF)
            {
                iRegValue &= 0xFFFF;
                //iRegValue = 0xFFFF;
                m_aCR[m_iIL].SetHigh ();
                m_aCR[m_iIL].SetBinaryOverflow ();
                if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
            }
            else
            {
                m_aCR[m_iIL].SetLow ();
                if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
            }
            iRegValue &= 0xFFFF;

            if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL) == (byte)ERegisterMask.REGMSK_IAR_IL)
            {
                if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL1) == (byte)ERegisterMask.REGMSK_IAR_IL1)
                {
                    m_iaIAR[2] = iRegValue;
                    if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[2], 2)); }
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL2) == (byte)ERegisterMask.REGMSK_IAR_IL2)
                {
                    m_iaIAR[3] = iRegValue;
                    if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[3], 3)); }
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL3) == (byte)ERegisterMask.REGMSK_IAR_IL3)
                {
                    m_iaIAR[4] = iRegValue;
                    if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[4], 4)); }
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL4) == (byte)ERegisterMask.REGMSK_IAR_IL4)
                {
                    m_iaIAR[5] = iRegValue;
                    if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[5], 5)); }
                }
                else
                {
                    m_iaIAR[1] = iRegValue;
                    if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[1], 1)); }
                }
            }
            else
            {
                // Replace the register value
                if ((yQByte & (byte)ERegisterMask.REGMSK_PL2IAR) == (byte)ERegisterMask.REGMSK_PL2IAR)
                {
                    m_iIARpl2 = iRegValue;
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_PL1IAR) == (byte)ERegisterMask.REGMSK_PL1IAR)
                {
                    m_iIARpl1 = iRegValue;
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR) == (byte)ERegisterMask.REGMSK_IAR)
                {
                    m_iaIAR[m_iIL] = iRegValue;
                    if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[m_iIL], m_iIL)); }
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_ARR) == (byte)ERegisterMask.REGMSK_ARR)
                {
                    m_iaARR[m_iIL] = iRegValue;
                    if (m_objEmulatorState.IsSingleStep () && OnNewARR != null) { OnNewARR (this, new NewARREventArgs (m_iaARR[m_iIL], m_iIL)); }
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_PSR) == (byte)ERegisterMask.REGMSK_PSR)
                {
                    m_aCR[m_iIL].Load (iRegValue);
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_XR2) == (byte)ERegisterMask.REGMSK_XR2)
                {
                    m_iXR2 = iRegValue;
                    if (m_objEmulatorState.IsSingleStep () && OnNewXR2 != null) { OnNewXR2 (this, new NewXR2EventArgs (m_iXR2)); }
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_XR1) == (byte)ERegisterMask.REGMSK_XR1)
                {
                    m_iXR1 = iRegValue;
                    if (m_objEmulatorState.IsSingleStep () && OnNewXR1 != null) { OnNewXR1 (this, new NewXR1EventArgs (m_iXR1)); }
                }
            }

            if (GetShowChangedValues () &&
                iOldValue != iRegValue)
            {
                m_strInstructionAppendix = string.Format ("{0}: 0x{1:X4} --> 0x{2:X4}", strRegisterName, iOldValue, iRegValue);
            }
        }

        // L - Load Register
        private void ExecuteL (byte[] yaInstruction)
        {
            int    iOperandAddress = GetOneOperandAddress (yaInstruction);
            byte   yQByte          = yaInstruction[1];
            int    iOldValue       = 0;
            string strRegisterName = "";

            if (AddressWraps (iOperandAddress, 2))
            {
                AddressWraparound ();
                return;
            }

            // Replace the register value
            if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL) == (byte)ERegisterMask.REGMSK_IAR_IL)
            {
                if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL1) == (byte)ERegisterMask.REGMSK_IAR_IL1)
                {
                    iOldValue       = m_iaIAR[2];
                    strRegisterName = "IL 1 IAR"; // 5475 keyboard
                    m_iaIAR[2]       = LoadInt (iOperandAddress);
                    //if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[2], 2)); }
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL2) == (byte)ERegisterMask.REGMSK_IAR_IL2)
                {
                    iOldValue       = m_iaIAR[3];
                    strRegisterName = "IL 2 IAR"; // BSCA
                    m_iaIAR[3]       = LoadInt (iOperandAddress);
                    //if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[3], 3)); }
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL3) == (byte)ERegisterMask.REGMSK_IAR_IL3)
                {
                    iOldValue       = m_iaIAR[4];
                    strRegisterName = "IL 3 IAR";
                    m_iaIAR[4]       = LoadInt (iOperandAddress);
                    //if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[4], 4)); }
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL4) == (byte)ERegisterMask.REGMSK_IAR_IL4)
                {
                    iOldValue       = m_iaIAR[5];
                    strRegisterName = "IL 4 IAR"; // SIOC
                    m_iaIAR[5]       = LoadInt (iOperandAddress);
                    //if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[5], 5)); }
                }
                else
                {
                    iOldValue       = m_iaIAR[1];
                    strRegisterName = "IL 0 IAR"; // Main "thread", no interrupt needed
                    m_iaIAR[1]       = LoadInt (iOperandAddress);
                    //if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[1], 1)); }
                }
            }
            else
            {
                if ((yQByte & (byte)ERegisterMask.REGMSK_PL2IAR) == (byte)ERegisterMask.REGMSK_PL2IAR)
                {
                    iOldValue       = m_iIARpl2;
                    strRegisterName = "PL 2 IAR";
                    m_iIARpl2       = LoadInt (iOperandAddress);
                }
            
                if ((yQByte & (byte)ERegisterMask.REGMSK_PL1IAR) == (byte)ERegisterMask.REGMSK_PL1IAR)
                {
                    iOldValue       = m_iIARpl1;
                    strRegisterName = "PL 1 IAR";
                    m_iIARpl1       = LoadInt (iOperandAddress);
                }
            
                if ((yQByte & (byte)ERegisterMask.REGMSK_IAR) == (byte)ERegisterMask.REGMSK_IAR)
                {
                    iOldValue       = m_iaIAR[m_iIL];
                    strRegisterName = "IAR";
                    m_iaIAR[m_iIL]  = LoadInt (iOperandAddress);
                    if (m_iIL == IL_Main)
                    {
                        if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[m_iIL], m_iIL)); }

                        // Disassemble text program body
                        if (!m_bIsAbsoluteCardLoader                    &&
                            m_eUIRunMode == EUIRunMode.RUN_LoadFromText &&
                            m_iaIAR[IL_Main] != iOldValue               &&
                            m_iaIAR[IL_Main] > m_iHighAddressEC)
                        {
                            m_iOldStartDASMAddress = m_iLowAddressMI;
                            m_iOldEndDASMAddress   = m_iHighAddressMI;
                            m_iOldDASMEntryPoint   = m_iaIAR[IL_Main];
                            FireNewDisassemblyListingEvent (m_iOldStartDASMAddress, m_iOldEndDASMAddress, m_iOldDASMEntryPoint, m_iXR1, m_iXR2);
                        }
                    }
                }
            
                if ((yQByte & (byte)ERegisterMask.REGMSK_ARR) == (byte)ERegisterMask.REGMSK_ARR)
                {
                    iOldValue       = m_iaARR[m_iIL];
                    strRegisterName = "ARR";
                    m_iaARR[m_iIL]  = LoadInt (iOperandAddress);
                    if (m_objEmulatorState.IsSingleStep () && OnNewARR != null) { OnNewARR (this, new NewARREventArgs (m_iaARR[m_iIL], m_iIL)); }
                }
            
                if ((yQByte & (byte)ERegisterMask.REGMSK_PSR) == (byte)ERegisterMask.REGMSK_PSR)
                {
                    iOldValue       = m_aCR[m_iIL].Store ();
                    strRegisterName = string.Format ("IL {0} CR", m_iIL);
                    m_aCR[m_iIL].Load (LoadInt (iOperandAddress));
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
            
                if ((yQByte & (byte)ERegisterMask.REGMSK_XR2) == (byte)ERegisterMask.REGMSK_XR2)
                {
                    iOldValue       = m_iXR2;
                    strRegisterName = "XR2";
                    m_iXR2          = LoadInt (iOperandAddress);
                    if (m_objEmulatorState.IsSingleStep () && OnNewXR2 != null) { OnNewXR2 (this, new NewXR2EventArgs (m_iXR2)); }
                }
            
                if ((yQByte & (byte)ERegisterMask.REGMSK_XR1) == (byte)ERegisterMask.REGMSK_XR1)
                {
                    iOldValue       = m_iXR1;
                    strRegisterName = "XR1";
                    m_iXR1          = LoadInt (iOperandAddress);
                    if (m_objEmulatorState.IsSingleStep () && OnNewXR1 != null) { OnNewXR1 (this, new NewXR1EventArgs (m_iXR1)); }
                }
            }

            if (GetShowChangedValues ())
            {
                int iNewValue = LoadInt (iOperandAddress);
                if (iNewValue != iOldValue)
                {
                    m_strInstructionAppendix = string.Format ("{0}: 0x{1:X4} --> 0x{2:X4}", strRegisterName, iOldValue, iNewValue);
                }
            }
        }

        // LA - Load Address
        private void ExecuteLA (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];

            if ((yQByte & 0x01) == 0x01)
            {
                m_iXR1 = iOperandAddress;
                if (m_objEmulatorState.IsSingleStep () && OnNewXR1 != null) { OnNewXR1 (this, new NewXR1EventArgs (m_iXR1)); }
            }

            if ((yQByte & 0x02) == 0x02)
            {
                m_iXR2 = iOperandAddress;
                if (m_objEmulatorState.IsSingleStep () && OnNewXR2 != null) { OnNewXR2 (this, new NewXR2EventArgs (m_iXR2)); }
            }
        }

        // ST - Store Register
        private void ExecuteST (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];

            if (AddressWraps (iOperandAddress, 2))
            {
                AddressWraparound ();
                return;
            }

            if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL) == (byte)ERegisterMask.REGMSK_IAR_IL)
            {
                if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL1) == (byte)ERegisterMask.REGMSK_IAR_IL1)
                {
                    StoreRegister (m_iaIAR[2], iOperandAddress);
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL2) == (byte)ERegisterMask.REGMSK_IAR_IL2)
                {
                    StoreRegister (m_iaIAR[3], iOperandAddress);
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL3) == (byte)ERegisterMask.REGMSK_IAR_IL3)
                {
                    StoreRegister (m_iaIAR[4], iOperandAddress);
                }
                else if ((yQByte & (byte)ERegisterMask.REGMSK_IAR_IL4) == (byte)ERegisterMask.REGMSK_IAR_IL4)
                {
                    StoreRegister (m_iaIAR[5], iOperandAddress);
                }
                else
                {
                    StoreRegister (m_iaIAR[1], iOperandAddress);
                }
            }
            else
            {
                // Extract the register value
                if ((yQByte & (byte)ERegisterMask.REGMSK_PL2IAR) == (byte)ERegisterMask.REGMSK_PL2IAR)
                {
                    //NoPL2Support ();
                    //return;
                    StoreRegister (m_iIARpl2, iOperandAddress);
                }
                //else if ((yQByte & (byte)ERegType.REG_PL1IAR) == (byte)ERegType.REG_PL1IAR)
                else if (yQByte == (byte)ERegisterMask.REGMSK_PL1IAR)
                {
                    StoreRegister (m_iIARpl1, iOperandAddress);
                }
                //else if ((yQByte & (byte)ERegType.REG_IAR) == (byte)ERegType.REG_IAR)
                else if (yQByte == (byte)ERegisterMask.REGMSK_IAR)
                {
                    StoreRegister (m_iaIAR[m_iIL], iOperandAddress);
                }
                //else if ((yQByte & (byte)ERegType.REG_ARR) == (byte)ERegType.REG_ARR)
                else if (yQByte == (byte)ERegisterMask.REGMSK_ARR)
                {
                    StoreRegister (m_iaARR[m_iIL], iOperandAddress);
                }
                //else if ((yQByte & (byte)ERegType.REG_PSR) == (byte)ERegType.REG_PSR)
                else if (yQByte == (byte)ERegisterMask.REGMSK_PSR)
                {
                    StoreRegister (m_aCR[m_iIL].Store (), iOperandAddress);
                }
                //else if ((yQByte & (byte)ERegType.REG_XR2) == (byte)ERegType.REG_XR2)
                else if (yQByte == (byte)ERegisterMask.REGMSK_XR2)
                {
                    StoreRegister (m_iXR2, iOperandAddress);
                }
                //else if ((yQByte & (byte)ERegType.REG_XR1) == (byte)ERegType.REG_XR1)
                else if (yQByte == (byte)ERegisterMask.REGMSK_XR1)
                {
                    StoreRegister (m_iXR1, iOperandAddress);
                }
                else
                {
                    // Store multiple registers; OR them together
                    short sReturn = 0;

                    if ((yQByte & (byte)ERegisterMask.REGMSK_PL1IAR) > 0)
                        sReturn |= (short)m_iIARpl1;
                    if ((yQByte & (byte)ERegisterMask.REGMSK_IAR) > 0)
                        sReturn |= (short)m_iaIAR[m_iIL];
                    if ((yQByte & (byte)ERegisterMask.REGMSK_ARR) > 0)
                        sReturn |= (short)m_iaARR[m_iIL];
                    if ((yQByte & (byte)ERegisterMask.REGMSK_PSR) > 0)
                        sReturn |= (short)(m_aCR[m_iIL].Store ());
                    if ((yQByte & (byte)ERegisterMask.REGMSK_XR2) > 0)
                        sReturn |= (short)m_iXR2;
                    if ((yQByte & (byte)ERegisterMask.REGMSK_XR1) > 0)
                        sReturn |= (short)m_iXR1;

                    StoreRegister (sReturn, iOperandAddress);
                }
            }
        }

        // BC - Branch On Condition
        private void ExecuteBC (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }

            byte yQByte = yaInstruction[1];

            if (m_aCR[m_iIL].IsNoOp (yQByte))
            {
                m_iaARR[m_iIL] = iOperandAddress;
                if (m_objEmulatorState.IsSingleStep () && OnNewARR != null) { OnNewARR (this, new NewARREventArgs (m_iaARR[m_iIL], m_iIL)); }
            }
            else
            {
                if (m_aCR[m_iIL].IsConditionTrue (yQByte))
                {
                    m_iaARR[m_iIL] = m_iaIAR[m_iIL];
                    if (m_objEmulatorState.IsSingleStep () && OnNewARR != null) { OnNewARR (this, new NewARREventArgs (m_iaARR[m_iIL], m_iIL)); }

                    m_iaIAR[m_iIL] = iOperandAddress;
                    if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[m_iIL], m_iIL)); }
                }
                else
                {
                    m_iaARR[m_iIL] = iOperandAddress;
                    if (m_objEmulatorState.IsSingleStep () && OnNewARR != null) { OnNewARR (this, new NewARREventArgs (m_iaARR[m_iIL], m_iIL)); }
                }
            }

            // Disassembly after loading program binary image from IPL card series
            if (!m_bIsAbsoluteCardLoader                   &&
                !m_bNewDisassemblyListingEventCalled       &&
                m_eUIRunMode == EUIRunMode.RUN_LoadFromIPL &&
                m_iIPLCardCount > 1                        &&
                m_iaIAR[m_iIL] > CARD_IMAGE_IPL_SIZE)
            {
                // Set IAR to BC destination
                m_iOldStartDASMAddress = m_iNewStartDASMAddress;
                m_iOldEndDASMAddress   = m_iNewEndDASMAddress;
                m_iOldDASMEntryPoint   = m_iaIAR[m_iIL];
                m_iNewStartDASMAddress = 0x0000;
                m_iNewEndDASMAddress   = 0x0000;
                m_iNewDASMEntryPoint   = 0x0000;
                FireNewDisassemblyListingEvent (m_iOldStartDASMAddress, m_iOldEndDASMAddress, m_iOldDASMEntryPoint, m_iXR1, m_iXR2);
                //FireIPLProgramSizeEvent (m_iProgramSizeIPL);
            }

            // Disassembly for AbsoluteCareLoader
            if (m_bIsAbsoluteCardLoader  &&
                m_iIPLCardReadCount == 6 &&
                m_iaIAR[IL_Main]    == 0x007C)
            {
                m_iOldStartDASMAddress = 0x0060;
                m_iOldEndDASMAddress   = 0x00FF;
                m_iOldDASMEntryPoint   = m_iaIAR[m_iIL];
                FireNewDisassemblyListingEvent (m_iOldStartDASMAddress, m_iOldEndDASMAddress, m_iOldDASMEntryPoint, m_iXR1, m_iXR2);
                FireIPLProgramSizeEvent (m_iProgramSizeIPL);
            }

            // Disassemble text program body
            if (!m_bIsAbsoluteCardLoader                    &&
                m_eUIRunMode == EUIRunMode.RUN_LoadFromText &&
                m_iaIAR[IL_Main] > m_iHighAddressEC)
            {
                m_iOldStartDASMAddress = m_iLowAddressMI;
                m_iOldEndDASMAddress   = m_iHighAddressMI;
                m_iOldDASMEntryPoint   = m_iaIAR[m_iIL];
                FireNewDisassemblyListingEvent (m_iOldStartDASMAddress, m_iOldEndDASMAddress, m_iOldDASMEntryPoint, m_iXR1, m_iXR2);
            }
        }

        // LIO - Load I/O
#if TEST_ONE_CARD_CLOCK_PROGRAM
        private DateTime m_dtLast;
        private int m_iCount = 0;
        private int m_iTotal = 0;
#endif
        private void ExecuteLIO (byte[] yaInstruction)
        {
            int  iOperandAddress = GetOneOperandAddress (yaInstruction);
            byte yQByte          = yaInstruction[1],
                 yDeviceAddress  = (byte)(yQByte & 0xF0),
                 yMCode          = (byte)(yQByte & 0x08),
                 yNCode          = (byte)(yQByte & 0x07);

            if (AddressWraps (iOperandAddress, 2))
            {
                AddressWraparound ();
                return;
            }

            if (yDeviceAddress == (byte)EIODevice.DEV_5424_MFCU)
            {
                if ((yQByte & 0x08) != 0x08) // Diagnostic mode not supported since I have no idea what it does
                {
                    if ((yQByte & 0x07) == 0x04)
                    {
                        //m_i5424PrintDataAddressRegister = LoadInt (iOperandAddress);
                        m_obj5424MFCU.i5424PrintDAR = LoadInt (iOperandAddress);
                        //m_strLIODetails = string.Format ("            MFCU Print DAR after LIO: 0x{0:X4}", m_i5424PrintDataAddressRegister);
                        m_strLIODetails = string.Format ("            MFCU Print DAR after LIO: 0x{0:X4}", m_obj5424MFCU.i5424PrintDAR);
                        if (m_objEmulatorState.IsSingleStep () && OnNewMPDAR != null) { OnNewMPDAR (this, new NewMPDAREventArgs (m_obj5424MFCU.i5424PrintDAR)); }
                    }
                    else if ((yQByte & 0x07) == 0x05)
                    {
                        //m_i5424ReadDataAddressRegister = LoadInt (iOperandAddress);
                        m_obj5424MFCU.i5424ReadDAR = LoadInt (iOperandAddress);
                        //m_strLIODetails = string.Format ("            MFCU Read DAR after LIO: 0x{0:X4}", m_i5424ReadDataAddressRegister);
                        m_strLIODetails = string.Format ("            MFCU Read DAR after LIO: 0x{0:X4}", m_obj5424MFCU.i5424ReadDAR);

                        if (m_eUIRunMode == EUIRunMode.RUN_LoadFromIPL &&
                            m_iIPLCardCount > 1)
                        {
                            int iTargetBeginAddress = m_obj5424MFCU.i5424ReadDAR;
                            int iTargetEndAddress   = m_obj5424MFCU.i5424ReadDAR + CARD_IMAGE_IPL_SIZE;

                            if (iTargetBeginAddress > m_iOldStartDASMAddress)
                            {
                                if (m_iNewStartDASMAddress == 0x0000)
                                {
                                    m_iNewStartDASMAddress = iTargetBeginAddress;
                                }
                                else
                                {
                                    m_iNewStartDASMAddress = Math.Min (m_iNewStartDASMAddress, iTargetBeginAddress);
                                }
                                m_iNewEndDASMAddress       = Math.Max (m_iNewEndDASMAddress,   iTargetEndAddress);
                            }
                            //m_iOldStartDASMAddress   = 0x0000;
                            //m_iOldEndDASMAddress     = CARD_IMAGE_SIZE;
                            //m_iOldDASMEntryPoint     = 0x0000;
                            //m_iNewStartDASMAddress   = 0x0000;
                            //m_iNewEndDASMAddress     = 0x0000;
                            //m_iNewDASMEntryPoint     = 0x0000;
                        }
                        if (m_objEmulatorState.IsSingleStep () && OnNewMRDAR != null) { OnNewMRDAR (this, new NewMRDAREventArgs (m_obj5424MFCU.i5424ReadDAR)); }
                    }
                    else if ((yQByte & 0x07) == 0x06)
                    {
                        //m_i5424PunchDataAddressRegister = LoadInt (iOperandAddress);
                        m_obj5424MFCU.i5424PunchDAR = LoadInt (iOperandAddress);
                        //m_strLIODetails = string.Format ("            MFCU Punch DAR after LIO: 0x{0:X4}", m_i5424PunchDataAddressRegister);
                        m_strLIODetails = string.Format ("            MFCU Punch DAR after LIO: 0x{0:X4}", m_obj5424MFCU.i5424PunchDAR);
                        if (m_objEmulatorState.IsSingleStep () && OnNewMUDAR != null) { OnNewMUDAR (this, new NewMUDAREventArgs (m_obj5424MFCU.i5424PunchDAR)); }
                    }
                    else
                    {
                        InvalidQByte ();
                        return;
                    }
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5203_Printer)
            {
                //m_obj5203LinePrinter.DeviceLIO (yOpCode, yQByte);

                if ((yQByte & 0x08) == 0x08)
                {
                    InvalidQByte ();
                    return;
                }
                else
                {
                    if ((yQByte & 0x07) == 0x00)
                    {
                        // Load forms length register
                        m_obj5203LinePrinter.FormLength = LoadInt (iOperandAddress);
                        m_strLIODetails = string.Format ("            Printer Form Length after LIO: 0x{0:X4}", m_obj5203LinePrinter.FormLength);
                        if (m_objEmulatorState.IsSingleStep () && OnNewLPFLR != null) { OnNewLPFLR (this, new NewLPFLREventArgs (m_obj5203LinePrinter.FormLength)); }
                    }
                    else if ((yQByte & 0x07) == 0x04)
                    {
                        // Load printer chain image address register
                        // 1. The 48-character set image must be in the 48 bytes having
                        //    low-order address bytes of 00 through 2F.
                        // 2. The 120-character set image must be in the 120 bytes with
                        //    low-order address bytes of 00 through 77.
                        //m_i5203ChainImageAddressRegister = LoadInt (iOperandAddress);
                        m_obj5203LinePrinter.i5203ChainImageAddressRegister = LoadInt (iOperandAddress);
                        //m_strLIODetails = string.Format ("            Printer Chain Image Address Register after LIO: 0x{0:X4}",
                        //                                 m_i5203ChainImageAddressRegister);
                        m_strLIODetails = string.Format ("            Printer Chain Image Address Register after LIO: 0x{0:X4}",
                                                         m_obj5203LinePrinter.i5203ChainImageAddressRegister);
                        //m_obj5203LinePrinter.ChainImage = LoadString (iOperandAddress & 0xFF00, iOperandAddress & 0x00FF);
                        if (m_objEmulatorState.IsSingleStep () && OnNewLPIAR != null) { OnNewLPIAR (this, new NewLPIAREventArgs (m_obj5203LinePrinter.i5203ChainImageAddressRegister)); }
                    }
                    else if ((yQByte & 0x07) == 0x06)
                    {
                        // Load printer data address register
                        // 3. The line printer data for 96 print positions (5203 only) must occupy
                        //    the 96 bytes with low-order address bytes of 7C through DB.
                        // 4. The line printer data for 120 print positions (5203 only) must occupy
                        //    the 120 bytes with low-order address bytes of 7C through F3.
                        // 5. The line printer data for 132 print positions must occupy the 132
                        //    bytes with low-order address bytes of 7C through FF.
                        //m_i5203DataAddressRegister = LoadInt (iOperandAddress);
                        m_obj5203LinePrinter.i5203PrintDAR = LoadInt (iOperandAddress);
                        //m_strLIODetails = string.Format ("            Printer DAR after LIO: 0x{0:X4}", m_i5203DataAddressRegister);
                        m_strLIODetails = string.Format ("            Printer DAR after LIO: 0x{0:X4}", m_obj5203LinePrinter.i5203PrintDAR);
                        if (m_objEmulatorState.IsSingleStep () && OnNewLPDAR != null) { OnNewLPDAR (this, new NewLPDAREventArgs (m_obj5203LinePrinter.i5203PrintDAR)); }
                    }
                    else
                    {
                        InvalidQByte ();
                        return;
                    }
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_1 ||
                     yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_2)
            {
                // LIO is not drive-sensitive, to either device address will work fine
                // LIO ignores the M code
                if ((yQByte & 0x07) == 0x03)
                {
                    // Reserved for CE use; ignore
                    return;
                }
                else if ((yQByte & 0x07) == 0x04)
                {
                    m_iDiskReadWriteAddressRegister = LoadInt (iOperandAddress);
                    m_strInstructionAppendix = string.Format ("  DRWAR: {0:X4}", m_iDiskReadWriteAddressRegister);
                    if (m_objEmulatorState.IsSingleStep () && OnNewDRWAR != null) { OnNewDRWAR (this, new NewDRWAREventArgs (m_iDiskReadWriteAddressRegister)); }
                }
                else if ((yQByte & 0x07) == 0x06)
                {
                    m_iDiskControlAddressRegister = LoadInt (iOperandAddress);
                    m_strInstructionAppendix = string.Format ("  DCAR: {0:X4}", m_iDiskControlAddressRegister);
                    if (m_objEmulatorState.IsSingleStep () && OnNewDCAR != null) { OnNewDCAR (this, new NewDCAREventArgs (m_iDiskControlAddressRegister)); }
                    LoadDiskControlField ();
                    if (IsInTrace () && GetShowChangedValues ())
                    {
                        m_strLIODetails = m_objDiskControlField.DisplayStates (false);
                    }
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_Keyboard)
            {
                // Since both 5471 and 5475 keyboards have the same device address, the
                // keyboard object will need to determine which keyboard model it is
                if (m_eKeyboard == EKeyboard.KEY_5471)
                {
                    // Only the printer is affected
                    if ((yQByte & 0x08) == 0x08) // Printer bit 1, N code 0
                    {
                        byte yLIOchar = (byte)((LoadInt (iOperandAddress) & 0xFF00) >> 8);
                        m_y5471PrinterOutput = ConvertEBCDICtoASCII (yLIOchar);
                    }
                }
                else if (m_eKeyboard == EKeyboard.KEY_5475)
                {
                    if (m_bTestOneCardClockProgram ||
                        m_bSimulateSystem3CpuTiming)
                    {
                        DateTime dtNow = DateTime.Now;
                        if (m_dtLast != null)
                        {
                            int iLastSecond = m_dtLast.Second;
                            int iThisSecond = dtNow.Second;
                            if (iThisSecond < iLastSecond)
                            {
                                iThisSecond += 60;
                            }
                            int iMS = 0;
                            if (m_iCount > 0)
                            {
                                iMS = (iThisSecond - iLastSecond) * 1000;
                                iMS += (dtNow.Millisecond - m_dtLast.Millisecond);
                            }

                            m_iCount++;
                            m_iTotal += iMS;
                            if (m_iCount > 1)// && m_iCount % 10 == 0)
                            {
                                WriteOutput (string.Format ("{0:D3}: Elapsed Time: {1:D3} milliseconds, average: {2:F3}",
                                                            m_iCount - 1, iMS, ((float)m_iTotal / (m_iCount - 1))),
                                             EOutputTarget.OUTPUT_PrinterPanel);
                            }

                            if (m_bTestOneCardClockProgram && iMS < 1000)
                            {
                                Thread.Sleep (1000 - iMS); // Used to approxomate IBM System/3 CPU speed
                            }
                        }
                        m_dtLast = DateTime.Now;
                    }

                    int iDisplayValue = LoadInt (iOperandAddress);
                    m_b5475_LIO_Program_1_Indicator = ((iDisplayValue & 0x00000100) > 0);
                    m_b5475_LIO_Program_2_Indicator = ((iDisplayValue & 0x00000001) > 0);
                    byte yLeftDisplay = (byte)(iDisplayValue >> 8),
                         yRightDisplay = (byte)(iDisplayValue & 0xFF);

                    if (m_bAsyncRun &&
                        OnNew5475Code != null)
                    {
                        StringBuilder sbHaltCode = new StringBuilder ();
                        sbHaltCode.Append (Get5475DisplayCode (yLeftDisplay));
                        sbHaltCode.Append (Get5475DisplayCode (yRightDisplay));
                        OnNew5475Code (this, new New5475CodeEventArgs (sbHaltCode.ToString ()));
                    }

                    List<string> strl5475Display = Get5475Display (yLeftDisplay, yRightDisplay);

                    //if (IsInTrace ())
                    //{
                    AppendStringList (strl5475Display, m_strl5475Buffer);
                    //}
                    //else
                    //{
                    //    WriteOutputLine ("");
                    //    //WriteOutputLine ("* * *  5 4 7 5  * * *");
                    //    //WriteOutputLine ("1 [ " + strl5475Display[0] + " ]");
                    //    //WriteOutputLine ("2 [ " + strl5475Display[1] + " ]");
                    //    //WriteOutputLine ("3 [ " + strl5475Display[2] + " ]");
                    //    //WriteOutputLine ("4 [ " + strl5475Display[3] + " ]");
                    //    //WriteOutputLine ("5 [ " + strl5475Display[4] + " ]");
                    //    WriteOutputLine ("* * *  5 4 7 5  * * *");
                    //    WriteOutputLine ("[ " + strl5475Display[0] + " ]");
                    //    WriteOutputLine ("[ " + strl5475Display[1] + " ]");
                    //    WriteOutputLine ("[ " + strl5475Display[2] + " ]");
                    //    WriteOutputLine ("[ " + strl5475Display[3] + " ]");
                    //    WriteOutputLine ("[ " + strl5475Display[4] + " ]");
                    //}
                    Show5475Status ();
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else
            {
                UnsupportedDevice ();
            }
        }

        // TIO - Test I/O and Branch
        private void ExecuteTIO (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }

            byte yQByte         = yaInstruction[1],
                 yDeviceAddress = (byte)(yQByte & 0xF0),
                 yMCode         = (byte)(yQByte & 0x08),
                 yNCode         = (byte)(yQByte & 0x07);
            bool bConditionMet  = false;

            if (yDeviceAddress == (byte)EIODevice.DEV_5410_CPU)
            {
                // Dual Programming Feature
                // Do nothing for now
                if ((yQByte & 0x03) == 0)
                {
                    // Cancel program level
                }
                else if ((yQByte & 0x03) == 1)
                {
                    // Load program level from MFCU
                }
                else if ((yQByte & 0x03) == 2)
                {
                    // Reserved
                }
                else if ((yQByte & 0x03) == 3)
                {
                    // Load program level from printer keyboard
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5424_MFCU)
            {
                if ((yQByte & 0x07) == 0) // Not ready / hopper empty
                {
                    //bConditionMet = (m_iMFCUStatus & (int)EMFCUStatusFlags.MFCU_Not_Ready) > 0;
                    //m_iMFCUStatus &= ~(int)EMFCUStatusFlags.MFCU_Not_Ready;
                    bConditionMet = ((yQByte & 0x08) == 0) ? m_obj5424MFCU.IsPrimaryNotReady () : m_obj5424MFCU.IsSecondaryNotReady ();
                }
                else
                {
                    if ((yQByte & 0x01) > 0) // Read/feed busy
                    {
                        //bConditionMet = (m_iMFCUStatus & (int)EMFCUStatusFlags.MFCU_Busy_Read_Feed) > 0;
                        //m_iMFCUStatus &= ~(int)EMFCUStatusFlags.MFCU_Busy_Read_Feed;
                        bConditionMet = m_obj5424MFCU.IsReadFeedBusy ();
                    }
                    if ((yQByte & 0x02) > 0) // Punch busy
                    {
                        //bConditionMet = (m_iMFCUStatus & (int)EMFCUStatusFlags.MFCU_Busy_Punch) > 0;
                        //m_iMFCUStatus &= ~(int)EMFCUStatusFlags.MFCU_Busy_Punch;
                        bConditionMet = m_obj5424MFCU.IsPunchBusy ();
                    }
                    if ((yQByte & 0x04) > 0) // Print busy
                    {
                        //bConditionMet = (m_iMFCUStatus & (int)EMFCUStatusFlags.MFCU_Busy_Print) > 0;
                        //m_iMFCUStatus &= ~(int)EMFCUStatusFlags.MFCU_Busy_Print;
                        bConditionMet = m_obj5424MFCU.IsPrintBusy ();
                    }
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5203_Printer)
            {
                if ((yQByte & 0x02) > 0) // Print buffer busy
                {
                    //bConditionMet = (m_iPrinterStatus & (int)EPrinterStatusFlags.PRINTER_Print_Buffer_Busy) > 0;
                    //m_iPrinterStatus &= ~(int)EPrinterStatusFlags.PRINTER_Print_Buffer_Busy;
                    bConditionMet = m_obj5203LinePrinter.IsPrintBufferBusy ();
                }
                if ((yQByte & 0x04) > 0) // Carriage busy
                {
                    //bConditionMet = (m_iPrinterStatus & (int)EPrinterStatusFlags.PRINTER_Carriage_Busy) > 0;
                    //m_iPrinterStatus &= ~(int)EPrinterStatusFlags.PRINTER_Carriage_Busy;
                    bConditionMet = m_obj5203LinePrinter.IsCarriageBusy ();
                }
                //if ((yQByte & 0x07) > 0x06) // Printer busy
                //{
                //    bConditionMet = (m_iPrinterStatus & (int)(EPrinterStatusFlags.PRINTER_Carriage_Busy |
                //                                              EPrinterStatusFlags.PRINTER_Print_Buffer_Busy)) > 0;
                //    m_iPrinterStatus &= ~(int)(EPrinterStatusFlags.PRINTER_Carriage_Busy |
                //                               EPrinterStatusFlags.PRINTER_Print_Buffer_Busy);
                //}
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_1)
            {
                if ((yQByte & 0x02) > 0) // Busy
                {
                    bConditionMet = (m_iDisk1Status & (int)EDiskStatusFlags.DISK_Busy) > 0;
                    m_iDisk1Status &= ~(int)EDiskStatusFlags.DISK_Busy;
                }
                if ((yQByte & 0x04) > 0) // Scan found
                {
                    bConditionMet = (m_iDisk1Status & (int)EDiskStatusFlags.DISK_Busy) > 0;
                    m_iDisk1Status &= ~(int)EDiskStatusFlags.DISK_Scan_Found;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_2)
            {
                if ((yQByte & 0x02) > 0) // Busy
                {
                    bConditionMet = (m_iDisk2Status & (int)EDiskStatusFlags.DISK_Busy) > 0;
                    m_iDisk2Status &= ~(int)EDiskStatusFlags.DISK_Busy;
                }
                if ((yQByte & 0x04) > 0) // Scan found
                {
                    bConditionMet = (m_iDisk2Status & (int)EDiskStatusFlags.DISK_Busy) > 0;
                    m_iDisk2Status &= ~(int)EDiskStatusFlags.DISK_Scan_Found;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_Keyboard)
            {
                // Not supported by either keyboard
                InvalidQByte ();
                return;
            }
            else
            {
                UnsupportedDevice ();
                return;
            }

            if ((m_eBootDevice == EBootDevice.BOOT_Disk_Fixed ||
                 m_eBootDevice == EBootDevice.BOOT_Disk_Removable) &&
                 m_bDebugDiskIPL)
            {
                if (!m_bFirstTime)
                {
                    bConditionMet = false;
                }
                else
                {
                    m_bFirstTime = false;
                }
            }

            if (bConditionMet)
            {
                m_iaARR[m_iIL] = m_iaIAR[m_iIL];
                if (m_objEmulatorState.IsSingleStep () && OnNewARR != null) { OnNewARR (this, new NewARREventArgs (m_iaARR[m_iIL], m_iIL)); }

                m_iaIAR[m_iIL] = iOperandAddress;
                if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[m_iIL], m_iIL)); }
            }
            else
            {
                m_iaARR[m_iIL] = iOperandAddress;
                if (m_objEmulatorState.IsSingleStep () && OnNewARR != null) { OnNewARR (this, new NewARREventArgs (m_iaARR[m_iIL], m_iIL)); }
            }
        }

        // SNS - Sense I/O
        private void ExecuteSNS (byte[] yaInstruction)
        {
            int  iOperandAddress = GetOneOperandAddress (yaInstruction);
            byte yQByte          = yaInstruction[1],
                 yDeviceAddress  = (byte)(yQByte & 0xF0),
                 yMCode          = (byte)(yQByte & 0x08),
                 yNCode          = (byte)(yQByte & 0x07);

            if (AddressWraps (iOperandAddress, 2))
            {
                AddressWraparound ();
                return;
            }

            if (yDeviceAddress == (byte)EIODevice.DEV_5410_CPU)
            {
                StoreRegister (m_usConsoleDialSetting, iOperandAddress);
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5424_MFCU)
            {
                if (yNCode == 0x00)
                {
                    // Special indicators for CE use - always zero for now
                    StoreRegister (0x0000, iOperandAddress);
                }
                else if (yNCode == 0x01)
                {
                    // Special indicators for CE use - always zero for now
                    StoreRegister (0x0000, iOperandAddress);
                }
                else if (yNCode == 0x03)
                {
                    // Status indicators - low-order byte for check conditions, always zero as virtual hardware never malfunctions
                    int iStatusBytePair = 0x0000;

                    //if (m_obj5242MFCU.IsPrintBuffer1Busy ())
                    //{
                    //    // Print Buffer 1 Busy
                    //    iStatusBytePair |= 0x8000;
                    //}
                    //if (m_obj5242MFCU.IsPrintBuffer2Busy ())
                    //{
                    //    // Print Buffer 2 Busy
                    //    iStatusBytePair |= 0x4000;
                    //}
                    if (m_obj5424MFCU.IsCardInPrimaryWaitStation ())
                    {
                        iStatusBytePair |= 0x2000;
                    }
                    if (m_obj5424MFCU.IsCardInSecondaryWaitStation ())
                    {
                        iStatusBytePair |= 0x1000;
                    }
                    //if (m_obj5242MFCU.IsHopperCycleNotComplete ())
                    //{
                    //    // Hopper Cycle Not Complete
                    //    iStatusBytePair |= 0x0400;
                    //}
                    //if (m_obj5242MFCU.GetTransportCounterBit2 ())
                    //{
                    //    // Card in Transport/Counter - bit 2
                    //    iStatusBytePair |= 0x0200;
                    //}
                    //if (m_obj5242MFCU.GetTransportCounterBit1 ())
                    //{
                    //    // Card in Transport/Counter - bit 1
                    //    iStatusBytePair |= 0x0100;
                    //}

                    StoreRegister (iStatusBytePair, iOperandAddress);
                }
                else if (yNCode == 0x04)
                {
                    // MFCU print data address register
                    //StoreRegister (m_i5424PrintDataAddressRegister, iOperandAddress);
                    StoreRegister (m_obj5424MFCU.i5424PrintDAR, iOperandAddress);
                }
                else if (yNCode == 0x05)
                {
                    // MFCU read data address register
                    //StoreRegister (m_i5424ReadDataAddressRegister, iOperandAddress);
                    StoreRegister (m_obj5424MFCU.i5424ReadDAR, iOperandAddress);
                }
                else if (yNCode == 0x06)
                {
                    // MFCU punch data address register
                    //StoreRegister (m_i5424PunchDataAddressRegister, iOperandAddress);
                    StoreRegister (m_obj5424MFCU.i5424PunchDAR, iOperandAddress);
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5203_Printer)
            {
                if (yNCode == 0x00)
                {
                    // Line position - left carriage is rightmost/low-order byte
                    StoreRegister (m_obj5203LinePrinter.LinePosition << 8, iOperandAddress);
                }
                else if (yNCode == 0x01)
                {
                    // In-print-operation details - always zero for now
                    StoreRegister (0x0000, iOperandAddress);
                }
                else if (yNCode == 0x02)
                {
                    // Printer timing - always zero for now
                    StoreRegister (0x0000, iOperandAddress);
                }
                else if (yNCode == 0x03)
                {
                    // Printer check status
                    StoreRegister (m_obj5203LinePrinter.GetStatusBytes (), iOperandAddress);
                }
                else if (yNCode == 0x04)
                {
                    // Printer 48-character chain image image address
                    //StoreRegister (m_i5203ChainImageAddressRegister, iOperandAddress);
                    StoreRegister (m_obj5203LinePrinter.i5203ChainImageAddressRegister, iOperandAddress);
                }
                else if (yNCode == 0x06)
                {
                    // Printer data address
                    //StoreRegister (m_i5203DataAddressRegister, iOperandAddress);
                    StoreRegister (m_obj5203LinePrinter.i5203PrintDAR, iOperandAddress);
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_1)
            {
                if (yNCode == 0x02)
                {
                    StoreRegister (m_iDiskStatusDrive1Bytes0and1, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskStatusDrive1Bytes0and1;
                }
                else if (yNCode == 0x03)
                {
                    StoreRegister (m_iDiskStatusDrive1Bytes2and3, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskStatusDrive1Bytes2and3;
                }
                else if (yNCode == 0x04)
                {
                    StoreRegister (m_iDiskReadWriteAddressRegister, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskReadWriteAddressRegister;
                }
                else if (yNCode == 0x06)
                {
                    StoreRegister (m_iDiskControlAddressRegister, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskControlAddressRegister;
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_2)
            {
                if (yNCode == 0x02)
                {
                    StoreRegister (m_iDiskStatusDrive2Bytes0and1, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskStatusDrive2Bytes0and1;
                }
                else if (yNCode == 0x03)
                {
                    StoreRegister (m_iDiskStatusDrive2Bytes2and3, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskStatusDrive2Bytes2and3;
                }
                else if (yNCode == 0x04)
                {
                    StoreRegister (m_iDiskReadWriteAddressRegister, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskReadWriteAddressRegister;
                }
                else if (yNCode == 0x06)
                {
                    StoreRegister (m_iDiskControlAddressRegister, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskControlAddressRegister;
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_Keyboard)
            {
                // Since both 5471 and 5475 keyboards have the same device address, the
                // keyboard object will need to determine which keyboard model it is.
                if (m_eKeyboard == EKeyboard.KEY_5471)
                {
                    if ((yQByte & 0x08) == 0x08)
                    {
                        // Return printer sense bytes
                        if ((yQByte & 0x07) == 0x01)
                        {
                            // Bytes 0 & 1 (useful bytes)
                            short s5471PrinterStatusBytes = 0;

                            // Load keyboard status flags
                            if ((m_i5471StatusFlags & STAT_Prt5471_Printer_Interrupt_Pending) > 0)
                            {
                                s5471PrinterStatusBytes |= k5471_SNS_PRT_Printer_Interrupt_Pending;
                            }

                            if ((m_i5471StatusFlags & STAT_Prt5471_Non_Printable_Character) > 0)
                            {
                                s5471PrinterStatusBytes |= k5471_SNS_PRT_Non_Printable_Character;
                            }

                            if ((m_i5471StatusFlags & STAT_Prt5471_Printer_Busy) > 0)
                            {
                                s5471PrinterStatusBytes |= k5471_SNS_PRT_Printer_Busy;
                            }

                            StoreRegister (s5471PrinterStatusBytes, iOperandAddress);
                        }
                        else if ((yQByte & 0x07) == 0x03)
                        {
                            // Bytes 2 & 3 (CE diagnostic bytes)
                            short s5471PrinterStatusBytes = 0;
                            StoreRegister (s5471PrinterStatusBytes, iOperandAddress);
                        }
                    }
                    else
                    {
                        // Return keyboard sense bytes
                        if ((yQByte & 0x07) == 0x01)
                        {
                            // Bytes 0 & 1 (useful bytes)
                            short s5471KeyboardStatusBytes = 0;

                            // Load keyboard character
                            if ((m_eBootDevice == EBootDevice.BOOT_Disk_Fixed ||
                                 m_eBootDevice == EBootDevice.BOOT_Disk_Removable) &&
                                 m_bDebugDiskIPL)
                            {
                                if (m_iTestSnsValueIdx < m_usaiTestSnsValues.Length)
                                {
                                    s5471KeyboardStatusBytes = (short)m_usaiTestSnsValues[m_iTestSnsValueIdx++];
                                }
                            }
                            else
                            {
                                s5471KeyboardStatusBytes = (short)(((short)m_y5471KeyboardInput) << 8);
                            }

                            // Load keyboard status flags
                            if ((m_i5471StatusFlags & STAT_Kbd5471_Request_Key_Interrupt_Enabled) > 0)
                            {
                                s5471KeyboardStatusBytes |= k5471_SNS_KBD_Request_Key_Interrupt_Pending;
                            }

                            if ((m_i5471StatusFlags & STAT_Kbd5471_End_Or_Cancel_Interrupt_Pending) > 0)
                            {
                                s5471KeyboardStatusBytes |= k5471_SNS_KBD_End_or_Cancel_Interrupt_Pending;
                            }

                            if ((m_i5471StatusFlags & STAT_Kbd5471_Cancel_Key) > 0)
                            {
                                s5471KeyboardStatusBytes |= k5471_SNS_KBD_Cancel_Key;
                            }

                            if ((m_i5471StatusFlags & STAT_Kbd5471_End_Key) > 0)
                            {
                                s5471KeyboardStatusBytes |= k5471_SNS_KBD_End_Key;
                            }

                            if ((m_i5471StatusFlags & STAT_Kbd5471_Return_Or_Data_Key_Interrupt_Pending) > 0)
                            {
                                s5471KeyboardStatusBytes |= k5471_SNS_KBD_Return_or_Data_Key_Interrupt_Pending;
                            }

                            if ((m_i5471StatusFlags & STAT_Kbd5471_Return_Key) > 0)
                            {
                                s5471KeyboardStatusBytes |= k5471_SNS_KBD_Return_Key;
                            }

                            StoreRegister (s5471KeyboardStatusBytes, iOperandAddress);
                        }
                        else if ((yQByte & 0x07) == 0x03)
                        {
                            // Bytes 2 & 3 (CE diagnostic bytes)
                            short s5471KeyboardStatusBytes = 0;

                            if ((m_i5471StatusFlags & STAT_Kbd5471_Request_Key_Interrupt_Enabled) > 0)
                            {
                                s5471KeyboardStatusBytes |= (short)k5471_SNS_KBD_Request_Key_Interrupt_Enabled;
                            }

                            if ((m_i5471StatusFlags & STAT_Kbd5471_Other_Interrupts_Enabled) > 0)
                            {
                                s5471KeyboardStatusBytes |= (short)k5471_SNS_KBD_Other_Key_Interrupt_Enabled;
                            }

                            if ((m_i5471StatusFlags & STAT_Kbd5471_End_Or_Cancel_Interrupt_Pending) > 0)
                            {
                                s5471KeyboardStatusBytes |= (short)k5471_SNS_KBD_End_or_Cancel_Interrupt_Pending; // Request or End or Cancel Key
                            }

                            if ((m_i5471StatusFlags & STAT_Kbd5471_End_Or_Cancel_Interrupt_Pending) > 0)
                            {
                                s5471KeyboardStatusBytes |= 0x04; // Request or End or Cancel Key Sampled
                            }

                            StoreRegister (s5471KeyboardStatusBytes, iOperandAddress);
                        }
                    }
                }
                else if (m_eKeyboard == EKeyboard.KEY_5475)
                {
                    if ((yQByte & 0x07) == 0x01)
                    {
                        // k5475_SNS_1L_Print_Switch_On               = 0x0080;
                        // k5475_SNS_1L_Lower_Shift_Key_Pressed       = 0x0020;
                        // k5475_SNS_1L_Invalid_Character_Detected    = 0x0010;
                        // k5475_SNS_1L_Multi_Punch_Interrupt         = 0x0004;
                        Reset5475Sns001 ();
                        //m_y5475KeyboardSNS001Low = 0;
                        if (m_b5475_SNS_1L_Lower_Shift_Key_Pressed)
                        {
                            m_y5475KeyboardSNS001Low |= k5475_SNS_1L_Lower_Shift_Key_Pressed;
                            //m_b5475_SNS_1L_Lower_Shift_Key_Pressed = false;
                        }
                        if (m_b5475_SNS_1L_Invalid_Character_Detected)
                        {
                            m_y5475KeyboardSNS001Low |= k5475_SNS_1L_Invalid_Character_Detected;
                            //m_b5475_SNS_1L_Invalid_Character_Detected = false;
                        }
                        if (m_e5475Interrupt == E5475Interrupt.E5475_Interrupt_Multi_Punch)
                        {
                            m_y5475KeyboardSNS001Low |= k5475_SNS_1L_Multi_Punch_Interrupt;
                            //m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_None;
                        }
                        if (m_e5475Interrupt == E5475Interrupt.E5475_Interrupt_Data_Key)
                        {
                            m_y5475KeyboardSNS001Low |= k5475_SNS_1L_Data_Key_Interrupt;
                            //m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_None;
                        }
                        short s5475SNS = (short)((m_y5475KeyboardInput << 8) | m_y5475KeyboardSNS001Low);
                        StoreRegister (s5475SNS, iOperandAddress);
                        Show5475Status ();
                    }
                    else if ((yQByte & 0x07) == 0x02)
                    {
                        // k5475_SNS_2H_Program_Load_Switch_Operated  = 0x0020;           
                        // k5475_SNS_2L_Auto_Skip_Dup_Switch_On       = 0x0080;
                        // k5475_SNS_2L_Record_Erase_Switch_Operated  = 0x0040;
                        // k5475_SNS_2L_Program_Switch_On             = 0x0010;
                        // k5475_SNS_2L_Auto_Record_Release_Switch_On = 0x0002;
                        Reset5475Sns010 ();
                        //m_y5475KeyboardSNS010High = 0;
                        if (m_b5475_SNS_2H_Program_1_Key_Pressed)
                        {
                            m_y5475KeyboardSNS010High |= k5475_SNS_2H_Program_1_Key_Pressed;
                            //m_b5475_SNS_2H_Program_1_Key_Pressed = false;
                        }
                        if (m_b5475_SNS_2H_Program_2_Key_Pressed)
                        {
                            m_y5475KeyboardSNS010High |= k5475_SNS_2H_Program_2_Key_Pressed;
                            //m_b5475_SNS_2H_Program_2_Key_Pressed = false;
                        }
                        if (m_b5475_SNS_2H_Release_Key_Pressed)
                        {
                            m_y5475KeyboardSNS010High |= k5475_SNS_2H_Release_Key_Pressed;
                            //m_b5475_SNS_2H_Release_Key_Pressed = false;
                        }
                        if (m_b5475_SNS_2H_Field_Erase_Key_Pressed)
                        {
                            m_y5475KeyboardSNS010High |= k5475_SNS_2H_Field_Erase_Key_Pressed;
                            //m_b5475_SNS_2H_Field_Erase_Key_Pressed = false;
                        }
                        if (m_b5475_SNS_2H_Error_Reset_Key_Pressed)
                        {
                            m_y5475KeyboardSNS010High |= k5475_SNS_2H_Error_Reset_Key_Pressed;
                            //m_b5475_SNS_2H_Error_Reset_Key_Pressed = false;
                        }
                        if (m_b5475_SNS_2H_Read_Key_Pressed)
                        {
                            m_y5475KeyboardSNS010High |= k5475_SNS_2H_Read_Key_Pressed;
                            //m_b5475_SNS_2H_Read_Key_Pressed = false;
                        }

                        //m_y5475KeyboardSNS010Low = 0;
                        if (m_b5475_SNS_2L_Record_Erase_Switch_Operated)
                        {
                            m_y5475KeyboardSNS010Low |= k5475_SNS_2L_Record_Erase_Switch_Operated;
                            //m_b5475_SNS_2L_Record_Erase_Switch_Operated = false;
                        }
                        if (m_b5475_SNS_2L_Program_Switch_On)
                        {
                            m_y5475KeyboardSNS010Low |= k5475_SNS_2L_Program_Switch_On;
                            //m_b5475_SNS_2L_Program_Switch_On = false;
                        }
                        if (m_b5475_SNS_2L_Skip_Key_Pressed)
                        {
                            m_y5475KeyboardSNS010Low |= k5475_SNS_2L_Skip_Key_Pressed;
                            //m_b5475_SNS_2L_Skip_Key_Pressed = false;
                        }
                        if (m_b5475_SNS_2L_Dup_Key_Pressed)
                        {
                            m_y5475KeyboardSNS010Low |= k5475_SNS_2L_Dup_Key_Pressed;
                            //m_b5475_SNS_2L_Dup_Key_Pressed = false;
                        }
                        if (m_b5475_SNS_2L_Auto_Record_Release_Switch_On)
                        {
                            m_y5475KeyboardSNS010Low |= k5475_SNS_2L_Auto_Record_Release_Switch_On;
                            //m_b5475_SNS_2L_Auto_Record_Release_Switch_On = false;
                        }
                        if (m_e5475Interrupt == E5475Interrupt.E5475_Interrupt_Function_Key)
                        {
                            m_y5475KeyboardSNS010Low |= k5475_SNS_2L_Function_Key_Interrupt;
                            //m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_None;
                        }
                                                             
                        short s5475SNS = (short)((m_y5475KeyboardSNS010High << 8) | m_y5475KeyboardSNS010Low);
                        StoreRegister (s5475SNS, iOperandAddress);
                        Show5475Status ();
                    }
                    else if ((yQByte & 0x07) == 0x03)
                    {
                        // SNS 011 High always 0x00
                        Reset5475Sns011 ();
                        //m_y5475KeyboardSNS011Low = 0;
                        if (m_b5475_SNS_3L_Keyboard_Interrupts_Enabled)
                        {
                            m_y5475KeyboardSNS011Low |= k5475_SNS_3L_Keyboard_Interrupts_Enabled;
                            //m_b5475_SNS_3L_Keyboard_Interrupts_Enabled = false;
                        }
                        if (m_b5475_SNS_3L_Any_Function_Key_Pressed)
                        {
                            m_y5475KeyboardSNS011Low |= k5475_SNS_3L_Any_Function_Key_Pressed;
                            //m_b5475_SNS_3L_Any_Function_Key_Pressed = false;
                        }
                        if (m_b5475_SNS_3L_Any_Data_Key)
                        {
                            m_y5475KeyboardSNS011Low |= k5475_SNS_3L_Any_Data_Key;
                            //m_b5475_SNS_3L_Any_Data_Key = false;
                        }
                        StoreRegister ((short)m_y5475KeyboardSNS011Low, iOperandAddress);
                        Show5475Status ();
                    }

                    m_strKeyStroke = "";
                    m_cKeyStroke   = '\x00';
                    m_bKeyCtrl     = false;
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else
            {
                UnsupportedDevice ();
            }
        }

        // HPL - Halt Program Level
        private void ExecuteHPL (byte[] yaInstruction)
        {
            m_iHaltCount++;

            byte yLeftDisplay  = yaInstruction[1],
                 yRightDisplay = yaInstruction[2];

            if (m_bAsyncRun &&
                OnNewHaltCode != null)
            {
                StringBuilder sbHaltCode = new StringBuilder ();
                sbHaltCode.Append (GetHaltDisplayCode (yLeftDisplay));
                sbHaltCode.Append (GetHaltDisplayCode (yRightDisplay));
                OnNewHaltCode (this, new NewHaltCodeEventArgs (sbHaltCode.ToString ()));
                m_iHPL_IL = m_iIL;
            }

            if (m_bAsyncRun)
            {
                if (!m_objEmulatorState.IsSingleStep ())
                {
                    m_objEmulatorState.SetHalted (); // .ChangeState (CEmulatorState.EProgramState.PSTATE_4_ProgramHalted);

                    while (m_objEmulatorState.IsHalted ())
                    {
                        Thread.Sleep (250);
                        if (m_objEmulatorState.IsRunning ()    ||  // Enter key pressed to resume program execution
                            m_objEmulatorState.IsSingleStep ())
                        {
                            return;
                        }
                        else if (m_objEmulatorState.IsProgramUnloaded () ||
                                 m_objEmulatorState.IsAborted ()) // stop
                        {
                            throw new Exception (ESCAPE_KEY);
                        }
                    }
                }
            }
            //else if (m_bShow5475Halt) // For UnitTest / non-async
            //{
            //    List<string> strlHaltDisplay = GetHaltDisplay (yLeftDisplay, yRightDisplay);

            //    //if (IsInTrace ())
            //    //{
            //    AppendStringList (strlHaltDisplay, m_strlHplBuffer);
            //    //}
            //    //else
            //    //{
            //    //    WriteOutputLine ("");
            //    //    WriteOutputLine ("* * *  H A L T  * * *");
            //    //    //WriteOutputLine ("1 [ " + strlHaltDisplay[0] + " ]");
            //    //    //WriteOutputLine ("2 [ " + strlHaltDisplay[1] + " ]");
            //    //    //WriteOutputLine ("3 [ " + strlHaltDisplay[2] + " ]");
            //    //    //WriteOutputLine ("4 [ " + strlHaltDisplay[3] + " ]");
            //    //    //WriteOutputLine ("5 [ " + strlHaltDisplay[4] + " ]");
            //    //    WriteOutputLine ("    [ " + strlHaltDisplay[0] + " ]");
            //    //    WriteOutputLine ("    [ " + strlHaltDisplay[1] + " ]");
            //    //    WriteOutputLine ("    [ " + strlHaltDisplay[2] + " ]");
            //    //    WriteOutputLine ("    [ " + strlHaltDisplay[3] + " ]");
            //    //    WriteOutputLine ("    [ " + strlHaltDisplay[4] + " ]");

            //    //    //PrintStringListToConsole (strlHaltDisplay);
            //    //}
            //}

            //if (m_bStopOnHalt) // for UnitTest // non-async
            //{
            //    bool bStuck = true;
            //    while (bStuck)
            //    {
            //        Thread.Sleep (250);
            //    }
            //}
        }

        // APL - Advance Program Level
        private void ExecuteAPL (byte[] yaInstruction)
        {
            byte yQByte         = yaInstruction[1],
                 //yControlCode   = yaInstruction[2],
                 yDeviceAddress = (byte)(yQByte & 0xF0),
                 yMCode         = (byte)(yQByte & 0x08),
                 yNCode         = (byte)(yQByte & 0x07);

            if (yDeviceAddress == (byte)EIODevice.DEV_5410_CPU)
            {
                // Dual Programming Feature
                // Do nothing for now
                if ((yQByte & 0x03) == 0)
                {
                    // Cancel program level
                }
                else if ((yQByte & 0x03) == 1)
                {
                    // Load program level from MFCU
                }
                else if ((yQByte & 0x03) == 2)
                {
                    // Reserved
                }
                else if ((yQByte & 0x03) == 3)
                {
                    // Load program level from printer keyboard
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5424_MFCU)
            {
                if ((yQByte & 0x07) == 0) // Not ready / hopper empty
                {
                    if ((yQByte & 0x08) == 0)
                    {
                        m_obj5424MFCU.ResetPrimaryNotReady ();
                    }
                    else
                    {
                        m_obj5424MFCU.ResetSecondaryNotReady ();
                    }
                }
                else
                {
                    if ((yQByte & 0x01) > 0) // Read/feed busy
                    {
                        //m_iMFCUStatus &= ~(int)EMFCUStatusFlags.MFCU_Busy_Read_Feed;
                        m_obj5424MFCU.ResetReadFeedBusy ();
                    }
                    if ((yQByte & 0x02) > 0) // Punch busy
                    {
                        //m_iMFCUStatus &= ~(int)EMFCUStatusFlags.MFCU_Busy_Punch;
                        m_obj5424MFCU.ResetPunchBusy ();
                    }
                    if ((yQByte & 0x04) > 0) // Print busy
                    {
                        //m_iMFCUStatus &= ~(int)EMFCUStatusFlags.MFCU_Busy_Print;
                        m_obj5424MFCU.ResetPrintBusy ();
                    }
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5203_Printer)
            {
                if ((yQByte & 0x02) > 0) // Print buffer busy
                {
                    //m_iPrinterStatus &= ~(int)EPrinterStatusFlags.PRINTER_Print_Buffer_Busy;
                    m_obj5203LinePrinter.ResetPrintBufferBusy ();
                }
                if ((yQByte & 0x04) > 0) // Carriage busy
                {
                    //m_iPrinterStatus &= ~(int)EPrinterStatusFlags.PRINTER_Carriage_Busy;
                    m_obj5203LinePrinter.ResetCarriageBusy ();
                }
                //else if ((yQByte & 0x07) == 0x06) // Printer busy
                //{
                //    m_iPrinterStatus &= ~(int)(EPrinterStatusFlags.PRINTER_Carriage_Busy |
                //                               EPrinterStatusFlags.PRINTER_Print_Buffer_Busy);
                //}
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_1)
            {
                if ((yQByte & 0x02) > 0) // Busy
                {
                    m_iDisk1Status &= ~(int)EDiskStatusFlags.DISK_Busy;
                }
                if ((yQByte & 0x04) > 0) // Scan found
                {
                    m_iDisk1Status &= ~(int)EDiskStatusFlags.DISK_Scan_Found;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_2)
            {
                if ((yQByte & 0x02) > 0) // Busy
                {
                    m_iDisk2Status &= ~(int)EDiskStatusFlags.DISK_Busy;
                }
                if ((yQByte & 0x04) > 0) // Scan found
                {
                    m_iDisk2Status &= ~(int)EDiskStatusFlags.DISK_Scan_Found;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_Keyboard)
            {
                // Not supported by either keyboard
                InvalidQByte ();
                return;
            }
            else
            {
                UnsupportedDevice ();
            }
        }

        // JC - Jump On Condition
        private void ExecuteJC (byte[] yaInstruction)
        {
            byte yQByte = yaInstruction[1];
            byte yControlCode = yaInstruction[2];

            if (!m_aCR[m_iIL].IsNoOp (yQByte))
            {
                if (m_aCR[m_iIL].IsConditionTrue (yQByte))
                {
                    //m_iARR[m_iIL] = m_iIAR[m_iIL];
                    m_iaIAR[m_iIL] += yControlCode;
                    if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[m_iIL], m_iIL)); }

                    if (!IsAddressValid (m_iaIAR[m_iIL]))
                    {
                        return;
                    }
                }
            }
        }

        // SIO - Start I/O
        private void ExecuteSIO (byte[] yaInstruction)
        {
            byte yQByte         = yaInstruction[1],
                 yControlCode   = yaInstruction[2],
                 yDeviceAddress = (byte)(yQByte & 0xF0),
                 yMCode         = (byte)(yQByte & 0x08),
                 yNCode         = (byte)(yQByte & 0x07);

            if (yDeviceAddress == (byte)EIODevice.DEV_5410_CPU)
            {
                // Dual Programming Feature
                // Do nothing for now
                if ((yControlCode & 0x04) > 0)
                {
                    // Enable DPF
                }
                else
                {
                    // Disable DPF
                }

                if ((yControlCode & 0x02) > 0)
                {
                    // Enable IL0 Interrupt Button
                }
                else
                {
                    // Disable IL0 Interrupt Button
                }

                if ((yControlCode & 0x01) > 0)
                {
                    // Reset Interrupt Request 0
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5424_MFCU)
            {
                //m_iMFCUStatus = 0; // Reset all device-is-busy flags
                m_obj5424MFCU.ResetAllMFCU ();

                bool bPrimaryHopper   = ((yQByte & 0x08) != 0x08),
                     bReadAction      = ((yQByte & 0x01) == 0x01),
                     bPunchAction     = ((yQByte & 0x02) == 0x02),
                     bPrintAction     = ((yQByte & 0x04) == 0x04),
                     bReadIPL         = ((yControlCode & 0x40) == 0x40),
                     bPrintFourthLine = ((yControlCode & 0x20) == 0x20);
                int iStackerNumber    = (int)(yControlCode & 0x07);
                if (iStackerNumber > 4)
                {
                    iStackerNumber -= 4;
                }
                else if (iStackerNumber != 4)
                {
                    iStackerNumber = bPrimaryHopper ? 1 : 4;
                }

                if (bReadAction)
                {
                    if (bReadIPL)
                    {
                        //LoadBinaryImage ((bPrimaryHopper ? m_obj5242MFCU.ReadCardFromPrimaryIPL () : m_obj5242MFCU.ReadCardFromSecondaryIPL ()),
                        //                 m_i5424ReadDataAddressRegister, 0);
                        LoadBinaryImage ((bPrimaryHopper ? m_obj5424MFCU.ReadCardFromPrimaryIPL () : m_obj5424MFCU.ReadCardFromSecondaryIPL ()),
                                         m_obj5424MFCU.i5424ReadDAR, 0);
                        ++m_iIPLCardReadCount;
                    }
                    else
                    {
                        string strCardImage = (bPrimaryHopper ? m_obj5424MFCU.ReadCardFromPrimary () : m_obj5424MFCU.ReadCardFromSecondary ());
                        if (strCardImage == null)
                        {
                            if (bPrimaryHopper)
                            {
                                m_obj5424MFCU.SetPrimaryNotReady ();
                            }
                            else
                            {
                                m_obj5424MFCU.SetSecondaryNotReady ();
                            }
                        }
                        else
                        {
                            //StoreString (strCardImage, m_i5424ReadDataAddressRegister);
                            StoreString (strCardImage, m_obj5424MFCU.i5424ReadDAR);
                        }
                    }

                    if (GetShowMFCUBuffers ())
                    {
                        if (m_objEmulatorState.IsSingleStep ())
                        {
                            m_strlIOBuffer.Add ("");
                        }
                        //m_strlIOBuffer.Add (string.Format ("   >>>   MFCU Read Buffer (0x{0:X4}):", m_i5424ReadDataAddressRegister));
                        //AppendStringList (DumpMFCUBuffer (m_i5424ReadDataAddressRegister), m_strlIOBuffer);
                        if (!m_bMFCUBufferRulerShown)
                        {
                            m_strlIOBuffer.Add (string.Format ("   >>>   MFCU Read Buffer (0x{0:X4}):", m_obj5424MFCU.i5424ReadDAR));
                        }
                        AppendStringList (DumpMFCUBuffer (m_obj5424MFCU.i5424ReadDAR), m_strlIOBuffer);
                    }

                    if (GetShowCol61to64 ())
                    {
                        StringBuilder sbldCol61to64 = new StringBuilder ();
                        for (int iOffset = 60; iOffset < 64; ++iOffset)
                        {
                            //char cData = ConvertEbcdicToAsciiChar ((char)m_yaMainMemory[m_i5424ReadDataAddressRegister + iOffset]);
                            char cData = ConvertEbcdicToAsciiChar ((char)m_yaMainMemory[m_obj5424MFCU.i5424ReadDAR + iOffset]);
                            sbldCol61to64.Append (cData);
                        }
                        if (m_bCpuMemTesting)
                        {
                            m_strInstructionAppendix = "  Col 61-64: " + sbldCol61to64.ToString ();
                        }
                    }

                    //m_iMFCUStatus |= (int)EMFCUStatusFlags.MFCU_Busy_Read_Feed;
                    m_obj5424MFCU.SetReadFeedBusy ();
                }

                if (bPunchAction)
                {
                    //m_iMFCUStatus |= (int)EMFCUStatusFlags.MFCU_Busy_Punch;
                    m_obj5424MFCU.SetPunchBusy ();
                    if (GetShowMFCUBuffers ())
                    {
                        m_strlIOBuffer.Add ("");
                        //m_strlIOBuffer.Add (string.Format ("   >>>   MFCU Punch Buffer (0x{0:X4}):", m_i5424PunchDataAddressRegister));
                        //AppendStringList (DumpMFCUBuffer (m_i5424PunchDataAddressRegister), m_strlIOBuffer);
                        m_strlIOBuffer.Add (string.Format ("   >>>   MFCU Punch Buffer (0x{0:X4}):", m_obj5424MFCU.i5424PunchDAR));
                        AppendStringList (DumpMFCUBuffer (m_obj5424MFCU.i5424PunchDAR), m_strlIOBuffer);
                        m_obj5424MFCU.WritePunchOutput (iStackerNumber, GetCardImage (m_obj5424MFCU.i5424PunchDAR, 96));
                    }
                }

                if (bPrintAction)
                {
                    //m_iMFCUStatus |= (int)EMFCUStatusFlags.MFCU_Busy_Print;
                    m_obj5424MFCU.SetPrintBusy ();
                    if (GetShowMFCUBuffers ())
                    {
                        m_strlIOBuffer.Add ("");
                        //m_strlIOBuffer.Add (string.Format ("   >>>   MFCU Print Buffer (0x{0:X4}):", m_i5424PrintDataAddressRegister));
                        //AppendStringList (DumpMFCUBuffer (m_i5424PrintDataAddressRegister, bPrintFourthLine), m_strlIOBuffer);
                        m_strlIOBuffer.Add (string.Format ("   >>>   MFCU Print Buffer (0x{0:X4}):", m_obj5424MFCU.i5424PrintDAR));
                        AppendStringList (DumpMFCUBuffer (m_obj5424MFCU.i5424PrintDAR, bPrintFourthLine), m_strlIOBuffer);
                        m_obj5424MFCU.WritePrintOutput (iStackerNumber, GetCardImage (m_obj5424MFCU.i5424PunchDAR, 128));
                    }
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5203_Printer)
            {
                // Operation: This instruction can initiate either or both forms movement and
                // printing. If printing is specified, the data contained in the printer data area
                // of storage is printed as a single line, beginning at the address specified in
                // the line printer data address register. Unprintable characters and coded blanks
                // (hex 40) print as blanks. Unprintable characters set a  testable indicator and
                // remain In the data area. All positions in which characters are printed are set
                // to hex 40. If forms movement is specified, the printer spaces or skips to the
                // next print line as specified by the control code.
                //m_iPrinterStatus = 0; // Reset all device-is-busy flags
                m_obj5203LinePrinter.ResetAllPrinter ();

                if ((yQByte & 0x07) == 0x00) // Space only
                {
                    for (int iIdx = 0; iIdx < (int)yControlCode; iIdx++)
                    {
                        WriteOutput ("", EOutputTarget.OUTPUT_PrinterPanel);
                    }

                    m_obj5203LinePrinter.SkipLines (yControlCode > 3 ? 0 : (int)yControlCode);

                    //m_iPrinterStatus |= (int)EPrinterStatusFlags.PRINTER_Carriage_Busy;
                    m_obj5203LinePrinter.SetCarriageBusy ();
                }
                else if ((yQByte & 0x07) == 0x02) // Print and space
                {
                    if (GetShowPrinterBuffer ())
                    {
                        m_strlIOBuffer.Add ("");
                        string strPrintBufferLabel = "   >>>   Printer buffer: ";
                        //List<string> strlBuffer = FormatWrappedDataString (m_i5203DataAddressRegister, 
                        //                                                   m_i5203DataAddressRegister + m_obj5203LinePrinter.i5203LineWidth - 1,
                        //                                                   strPrintBufferLabel);
                        List<string> strlBuffer = FormatWrappedDataString (m_obj5203LinePrinter.i5203PrintDAR, 
                                                                           m_obj5203LinePrinter.i5203PrintDAR + m_obj5203LinePrinter.i5203LineWidth - 1,
                                                                           strPrintBufferLabel);

                        AppendStringList (MakeRulerLines (m_obj5203LinePrinter.i5203LineWidth, 0), strlBuffer);
                                                
                        StringBuilder strbBuffer = new StringBuilder ();
                        for (int iIdx = 0; iIdx < m_obj5203LinePrinter.i5203LineWidth; ++iIdx)
                        {
                            //strbBuffer.Append ((char)ConvertEBCDICtoASCII (m_yaMainMemory[m_i5203DataAddressRegister + iIdx]));
                            strbBuffer.Append ((char)ConvertEBCDICtoASCII (m_yaMainMemory[m_obj5203LinePrinter.i5203PrintDAR + iIdx]));
                        }
                        strlBuffer.Add (strbBuffer.ToString ());

                        AppendStringList (strlBuffer, m_strlIOBuffer);
                    }

                    WriteOutput (PrintEBCDIC (), EOutputTarget.OUTPUT_PrinterPanel);
                    for (int iIdx = 1; iIdx < (int)yControlCode; iIdx++)
                    {
                        WriteOutput ("", EOutputTarget.OUTPUT_PrinterPanel);
                    }

                    m_obj5203LinePrinter.SkipLines (yControlCode > 3 ? 0 : (int)yControlCode);
                    m_obj5203LinePrinter.ResetUnprintableCharacter ();

                    //m_iPrinterStatus |= (int)EPrinterStatusFlags.PRINTER_Carriage_Busy;
                    //m_iPrinterStatus |= (int)EPrinterStatusFlags.PRINTER_Print_Buffer_Busy;
                    m_obj5203LinePrinter.SetCarriageBusy ();
                    m_obj5203LinePrinter.SetPrintBufferBusy ();
                }
                else if ((yQByte & 0x07) == 0x04) // Skip only
                {
                    int iLinesToSkip = m_obj5203LinePrinter.SkipToLine ((int)yControlCode);

                    //m_iPrinterStatus |= (int)EPrinterStatusFlags.PRINTER_Carriage_Busy;
                    m_obj5203LinePrinter.SetCarriageBusy ();

                    while (iLinesToSkip-- > 0)
                    {
                        WriteOutput ("", EOutputTarget.OUTPUT_PrinterPanel);
                    }
                }
                else if ((yQByte & 0x07) == 0x06) // Print and skip
                {
                    if (GetShowPrinterBuffer ())
                    {
                        m_strlIOBuffer.Add ("");
                        string strPrintBufferLabel = "   >>>   Printer buffer: ";
                        //List<string> strlBuffer = FormatWrappedDataString (m_i5203DataAddressRegister,
                        //                                                   m_i5203DataAddressRegister + m_obj5203LinePrinter.i5203LineWidth - 1,
                        //                                                   strPrintBufferLabel);
                        List<string> strlBuffer = FormatWrappedDataString (m_obj5203LinePrinter.i5203PrintDAR,
                                                                           m_obj5203LinePrinter.i5203PrintDAR + m_obj5203LinePrinter.i5203LineWidth - 1,
                                                                           strPrintBufferLabel);

                        AppendStringList (MakeRulerLines (m_obj5203LinePrinter.i5203LineWidth, 0), strlBuffer);

                        StringBuilder strbBuffer = new StringBuilder ();
                        for (int iIdx = 0; iIdx < m_obj5203LinePrinter.i5203LineWidth; ++iIdx)
                        {
                            //strbBuffer.Append ((char)ConvertEBCDICtoASCII (m_yaMainMemory[m_i5203DataAddressRegister + iIdx]));
                            strbBuffer.Append ((char)ConvertEBCDICtoASCII (m_yaMainMemory[m_obj5203LinePrinter.i5203PrintDAR + iIdx]));
                        }
                        strlBuffer.Add (strbBuffer.ToString ());

                        AppendStringList (strlBuffer, m_strlIOBuffer);
                    }

                    WriteOutput (PrintEBCDIC (), EOutputTarget.OUTPUT_PrinterPanel);

                    int iLinesToSkip = m_obj5203LinePrinter.SkipToLine ((int)yControlCode);
                    m_obj5203LinePrinter.ResetUnprintableCharacter ();

                    //m_iPrinterStatus |= (int)EPrinterStatusFlags.PRINTER_Carriage_Busy;
                    //m_iPrinterStatus |= (int)EPrinterStatusFlags.PRINTER_Print_Buffer_Busy;
                    m_obj5203LinePrinter.SetCarriageBusy ();
                    m_obj5203LinePrinter.SetPrintBufferBusy ();

                    while (iLinesToSkip-- > 0)
                    {
                        WriteOutput ("", EOutputTarget.OUTPUT_PrinterPanel);
                    }
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_1 ||
                     yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_2)
            {
                if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_1)
                {
                    m_iDisk1Status = 0;
                }
                else
                {
                    m_iDisk2Status = 0;
                }

                LoadDiskControlField ();
                m_objDiskControlField.SaveState ();

                // Reset Status Address A and Status Address B with each diks system SIO
                m_iDiskStatusDrive1Bytes0and1 &= ~(m_ki5444Byte1Bit6StatusAddressA | m_ki5444Byte1Bit7StatusAddressB);

                // Determine target from SIO
                bool bPrimaryDisk = (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_1);
                bool bFixedDisk   = (yMCode > 0);

                // Reset SNS status bits that are reset by SIO
                m_iDiskStatusDrive1Bytes0and1 &= ~m_ki5444Byte0Bit0NoOp;
                m_iDiskStatusDrive2Bytes0and1 &= ~m_ki5444Byte0Bit0NoOp;
                m_iDiskStatusDrive1Bytes0and1 &= ~m_ki5444Byte1Bit2EndOfCylinder;
                m_iDiskStatusDrive2Bytes0and1 &= ~m_ki5444Byte1Bit2EndOfCylinder;

                if (yNCode == 0) // Seek
                {
                    if (bPrimaryDisk)
                    {
                        m_bDrive1Head0Selection = ((m_objDiskControlField.yS & 0x80) == 0); // Head selection
                        m_iDisk1Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                    else // Drive 2
                    {
                        m_bDrive2Head0Selection = ((m_objDiskControlField.yS & 0x80) == 0); // Head selection
                        m_iDisk2Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }

                    if ((m_objDiskControlField.yS & 0x01) == 0 && // Toward cylinder 0
                        m_objDiskControlField.yN >= 224)          // Recalibration value
                    {
                        if (bPrimaryDisk)
                        {
                            m_iDrive1Cylinder = 0;
                            m_iDiskStatusDrive1Bytes0and1 |= m_ki5444Byte1Bit1CylinderZero;
                            m_bDrive1Head0Selection = false; // Head 0 always set by recallibration
                        }
                        else // Drive 2
                        {
                            m_iDrive2Cylinder = 0;
                            m_iDiskStatusDrive2Bytes0and1 |= m_ki5444Byte1Bit1CylinderZero;
                            m_bDrive2Head0Selection = false; // Head 0 always set by recallibration
                        }
                    }
                    else
                    {
                        if ((m_objDiskControlField.yS & 0x01) == 0) // Toward cylinder 0
                        {
                            if (bPrimaryDisk)
                            {
                                // Reset the "at cylinder 0" bit
                                m_iDiskStatusDrive1Bytes0and1 &= ~m_ki5444Byte1Bit1CylinderZero;

                                m_iDrive1Cylinder -= m_objDiskControlField.yN;
                                if (m_iDrive1Cylinder <= 0)
                                {
                                    m_iDrive1Cylinder = 0;
                                    m_iDiskStatusDrive1Bytes0and1 |= m_ki5444Byte1Bit1CylinderZero;
                                }
                                m_iDisk1Status |= (int)EDiskStatusFlags.DISK_Busy;
                            }
                            else // Drive 2
                            {
                                // Reset the "at cylinder 0" bit
                                m_iDiskStatusDrive2Bytes0and1 &= ~m_ki5444Byte1Bit1CylinderZero;

                                m_iDrive2Cylinder -= m_objDiskControlField.yN;
                                if (m_iDrive2Cylinder <= 0)
                                {
                                    m_iDrive2Cylinder = 0;
                                    m_iDiskStatusDrive2Bytes0and1 |= m_ki5444Byte1Bit1CylinderZero;
                                }
                                m_iDisk2Status |= (int)EDiskStatusFlags.DISK_Busy;
                            }
                        }
                        else // Away from cylinder 0
                        {
                            if (bPrimaryDisk)
                            {
                                // Reset the "at cylinder 0" bit
                                m_iDiskStatusDrive1Bytes0and1 &= ~m_ki5444Byte1Bit1CylinderZero;

                                m_iDrive1Cylinder += m_objDiskControlField.yN;
                                if (m_iDrive1Cylinder > 203)
                                {
                                    m_iDrive1Cylinder = 0;
                                    m_iDiskStatusDrive1Bytes0and1 |= m_ki5444Byte0Bit7SeekCheck;
                                }
                            }
                            else // Drive 2
                            {
                                // Reset the "at cylinder 0" bit
                                m_iDiskStatusDrive2Bytes0and1 &= ~m_ki5444Byte1Bit1CylinderZero;

                                m_iDrive2Cylinder += m_objDiskControlField.yN;
                                if (m_iDrive2Cylinder > 203)
                                {
                                    m_iDrive2Cylinder = 0;
                                    m_iDiskStatusDrive2Bytes0and1 |= m_ki5444Byte0Bit7SeekCheck;
                                }
                            }
                        }
                    }

                    if (GetShowDisassembly ())
                    {
                        m_strSIODetails = string.Format ("SIO   Seek to cylinder {0:D}, head {1:D}",
                                                          bPrimaryDisk ? m_iDrive1Cylinder : m_iDrive2Cylinder,
                                                         ((yQByte & 0x80) == 0) ? 0 : 1);
                    }
                }
                else if (yNCode == 1 && (yControlCode & 0x03) == 0 || // Read Data
                         yNCode == 2 && (yControlCode & 0x01) == 0)   // Write Data
                {
                    // Use the same code for reading and for writing, since the only difference is the
                    // direction of the data flow.  Address calculation and all else is identical.
                    bool bWrite = (yNCode == 2 && (yControlCode & 0x01) == 0);
                    int iSectorNumber = (m_objDiskControlField.yS >> 2) & 0x3F;
                    int iCylinder = bPrimaryDisk ? m_iDrive1Cylinder : m_iDrive2Cylinder;

                    // Error detection
                    if ((iSectorNumber > 23 &&
                         iSectorNumber < 32) ||
                        iSectorNumber > 55)
                    {
                        // Invalid sector #
                        if (bPrimaryDisk)
                        {
                            m_iDiskStatusDrive1Bytes0and1 |= m_ki5444Byte0Bit5NoRecordFound;
                        }
                        else
                        {
                            m_iDiskStatusDrive2Bytes0and1 |= m_ki5444Byte0Bit5NoRecordFound;
                        }

                        m_strSIODetails = string.Format ("SIO   Read invalid sector {0:D}", iSectorNumber);
                        return;
                    }
                    if (iCylinder > 203)
                    {
                        // Invalid cylinder #
                        m_strSIODetails = string.Format ("SIO   Read invalid cyclinder {0:D}", iCylinder);
                        return;
                    }

                    if (bPrimaryDisk)
                    {
                        m_iDisk1Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                    else // Drive 2
                    {
                        m_iDisk2Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }

                    // Sector number 00 - 23, 32 - 55
                    int iRawSectorNumber = iSectorNumber;
                    if (iSectorNumber > 23)
                    {
                        iSectorNumber -= 8;
                    }

                    if (GetShowDisassembly ())
                    {
                        m_strSIODetails = string.Format ("SIO   Read {0:D} sectors starting at {1:D} on cylinder {2:D}",
                                                         m_objDiskControlField.yN + 1, iRawSectorNumber, iCylinder);
                    }

                    // Determine number of sectors to read
                    int iLastSectorToRead = m_objDiskControlField.yN + iSectorNumber;

                    m_objDumpData.iStart = m_iDiskReadWriteAddressRegister;

                    for (; iSectorNumber <= iLastSectorToRead; ++iSectorNumber)
                    {
                        if (iSectorNumber > 47 || // Already read last sector in cylinder
                            m_objDiskControlField.yN == 0xFF)  // Sector count indicates completion
                        {
                            // Reached end of cylinder before fulfilling read request
                            if (m_objDiskControlField.yN != 0xFF)
                            {
                                if (bPrimaryDisk)
                                {
                                    m_iDiskStatusDrive1Bytes0and1 |= m_ki5444Byte1Bit2EndOfCylinder;
                                }
                                else
                                {
                                    m_iDiskStatusDrive2Bytes0and1 |= m_ki5444Byte1Bit2EndOfCylinder;
                                }
                            }
                            break;
                        }

                        // Determine starting offset in disk image file
                        int iFileOffset = (BYTES_PER_CYLINDER * iCylinder) + (SECTOR_SIZE * iSectorNumber);

                        int iNewSectorNumber = iSectorNumber + ((iSectorNumber > 23) ? 8 : 0);
                        m_objDiskControlField.yS = (byte)((iNewSectorNumber << 2) & 0x00FF); // S field contains ID of last sector read (p.6-12)
                        --m_objDiskControlField.yN; // Decrement N byte at the start of each sector

                        // Copy data to destination
                        byte[] yaSector = null;
                        if (bWrite)
                        {
                            yaSector = new byte[SECTOR_SIZE];

                            for (int iIdx = 0; iIdx < SECTOR_SIZE; ++iIdx)
                            {
                                yaSector[iIdx] = m_yaMainMemory[m_iDiskReadWriteAddressRegister + iIdx];
                            }

                            m_obj5444DiskDrive.WriteSector (iFileOffset, bFixedDisk, bPrimaryDisk, yaSector);
                        }
                        else
                        {
                            yaSector = m_obj5444DiskDrive.ReadSector (iFileOffset, bFixedDisk, bPrimaryDisk);

                            for (int iByteIdx = 0; iByteIdx < SECTOR_SIZE; ++iByteIdx)
                            {
                                m_yaMainMemory[m_iDiskReadWriteAddressRegister + iByteIdx] = yaSector[iByteIdx];
                            }
                        }

                        if (GetShowDiskBuffers ())
                        {
                            string strIOBuffer = (string.Format ("{0} Cylinder: {1}  Sector: {2} ({3})  File Offset: 0x{4:X8}  Destination: 0x{5:X8}, {6} {7}",
                                                                 bWrite ? "Write" : "Read", iCylinder, iSectorNumber, iRawSectorNumber, iFileOffset,
                                                                 m_iDiskReadWriteAddressRegister, bPrimaryDisk ? "Drive 1" : "Drive 2",
                                                                 bFixedDisk ? "Fixed" : "Removeable"));
                            m_strlIOBuffer.Add (strIOBuffer);
                            List<string> strlSectorDump = BinaryToDump (yaSector, 0, yaSector.Length - 1, iFileOffset);
                            AppendStringList (strlSectorDump, m_strlIOBuffer);
                        }

                        // Update Disk Read/Write Address Register
                        m_iDiskReadWriteAddressRegister += SECTOR_SIZE;
                        if (m_objEmulatorState.IsSingleStep () && OnNewDRWAR != null) { OnNewDRWAR (this, new NewDRWAREventArgs (m_iDiskReadWriteAddressRegister)); }
                        m_objDumpData.iLength += SECTOR_SIZE;
                    }
                }
                else if (yNCode == 1 && (yControlCode & 0x03) == 1) // Read Identifier
                {
                    if (bPrimaryDisk)
                    {
                        m_iDisk1Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                    else // Drive 2
                    {
                        m_iDisk2Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                }
                else if (yNCode == 1 && (yControlCode & 0x03) == 2) // Read Data Diagnostic
                {
                    if (bPrimaryDisk)
                    {
                        m_iDisk1Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                    else // Drive 2
                    {
                        m_iDisk2Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                }
                else if (yNCode == 1 && (yControlCode & 0x03) == 3) // Verify
                {
                    if (bPrimaryDisk)
                    {
                        m_iDisk1Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                    else // Drive 2
                    {
                        m_iDisk2Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                }
                else if (yNCode == 2 && (yControlCode & 0x01) == 1) // Write Identifier
                {
                    if (bPrimaryDisk)
                    {
                        m_iDisk1Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                    else // Drive 2
                    {
                        m_iDisk2Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                }
                else if (yNCode == 3 && (yControlCode & 0x03) == 0) // Scan Equal
                {
                    if (bPrimaryDisk)
                    {
                        m_iDisk1Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                    else // Drive 2
                    {
                        m_iDisk2Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                }
                else if (yNCode == 3 && (yControlCode & 0x03) == 1) // Scan Low or Equal
                {
                    if (bPrimaryDisk)
                    {
                        m_iDisk1Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                    else // Drive 2
                    {
                        m_iDisk2Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                }
                else if (yNCode == 3 && (yControlCode & 0x02) == 2) // Scan High or Equal
                {
                    if (bPrimaryDisk)
                    {
                        m_iDisk1Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                    else // Drive 2
                    {
                        m_iDisk2Status |= (int)EDiskStatusFlags.DISK_Busy;
                    }
                }
                else
                {
                    InvalidQByte ();
                    return;
                }

                StoreDiskControlField ();
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_Keyboard)
            {
                if (m_eKeyboard == EKeyboard.KEY_None)
                {
                    // Keyboard gets set to EKeyboard.KEY_5471 at boot time when booting from disk
                    m_eKeyboard = EKeyboard.KEY_5475;
                }

                // Since both 5471 and 5475 keyboards have the same device address, the
                // keyboard object will need to determine which keyboard model it is.
                if (m_eKeyboard == EKeyboard.KEY_5471)
                {
                    if ((yQByte & 0x08) == 0x08)
                    {
                        // Command addresses the printer
                        if ((yControlCode & k5471_SIO_PRT_Start_Print) > 0)
                        {
                            // Start print
                            if (m_strb5471PrinterOutput.Length < m_i5471PrinterFormWidth)
                            {
                                m_strb5471PrinterOutput.Append ((char)m_y5471PrinterOutput);
                                if (!m_bShowKeyboardIndicatorStatus)
                                {
                                    Console.Write ((char)m_y5471PrinterOutput);
                                }

                                m_i5471StatusFlags |= STAT_Prt5471_Printer_Busy;
                                if (!m_bDebugDiskIPL)
                                {
                                    m_iResetBusyCount = m_iInstructionCount + BUSY_COUNT_WAIT;
                                }
                                if (m_strb5471PrinterOutput.Length > m_i5471PrinterFormWidth - 6)
                                {
                                    m_i5471StatusFlags |= STAT_Prt5471_End_Of_Line;
                                }
                            }
                            else
                            {
                                m_i5471StatusFlags |= STAT_Prt5471_End_Of_Line;
                            }
                        }

                        if ((yControlCode & k5471_SIO_PRT_Start_Carrier_Return) > 0)
                        {
                            // Start carrier return
                            if (m_bShowKeyboardIndicatorStatus)
                            {
                                m_strl5471Output.Add ("  5471:  " + m_strb5471PrinterOutput.ToString ());
                            }
                            else
                            {
                                WriteOutput ("", EOutputTarget.OUTPUT_5471Printer);
                            }
                            m_strb5471PrinterOutput.Clear ();
                            m_i5471StatusFlags |= STAT_Prt5471_Printer_Busy;
                            if (!m_bDebugDiskIPL)
                            {
                                m_iResetBusyCount = m_iInstructionCount + BUSY_COUNT_WAIT;
                            }
                        }

                        if ((yControlCode & k5471_SIO_PRT_Printer_Interrupt_Enable_Disable) > 0)
                        {
                            // Printer interrupt; l =enable, 0 =disable
                            m_i5471StatusFlags |= STAT_Prt5471_Printer_Interrupt_Enabled;
                        }
                        else
                        {
                            m_i5471StatusFlags &= ~STAT_Prt5471_Printer_Interrupt_Enabled;
                        }

                        if ((yControlCode & k5471_SIO_PRT_Reset_Printer_Interrupt) > 0)
                        {
                            // Reset printer interrupt
                            m_i5471StatusFlags &= ~STAT_Prt5471_Printer_Interrupt_Pending;
                            m_iIL = IL_Main; // Reset interrupt and return main program level
                        }
                    }
                    else
                    {
                        // Command addresses the keyboard
                        if ((yControlCode & k5471_SIO_KBD_Request_Pending_Indicator_On_or_Off) > 0)
                        {
                            // Request pending indicator; 1 = on, 0 = off
                            m_i5471StatusFlags |= STAT_Kbd5471_Request_Pending_Indicator_On;
                        }
                        else
                        {
                            m_i5471StatusFlags &= ~STAT_Kbd5471_Request_Pending_Indicator_On;
                        }

                        if ((yControlCode & k5471_SIO_KBD_Proceed_Indicator_On_or_Off) > 0)
                        {
                            // Proceed indicator; 1 = on, 0 = off
                            if ((m_i5471StatusFlags & STAT_Kbd5471_Proceed_Pending_Indicator_On) == 0)
                            {
                                m_i5471StatusFlags |= STAT_Kbd5471_Proceed_Pending_Indicator_On;
                                if (m_bShowKeyboardIndicatorStatus)
                                {
                                    m_strl5471Output.Add ("  5471: <PROCEED ON>");
                                }
                                else
                                {
                                    Console.CursorSize = 10;
                                }
                            }
                        }
                        else
                        {
                            if ((m_i5471StatusFlags & STAT_Kbd5471_Proceed_Pending_Indicator_On) > 0)
                            {
                                m_i5471StatusFlags &= ~STAT_Kbd5471_Proceed_Pending_Indicator_On;
                                if (m_bShowKeyboardIndicatorStatus)
                                {
                                    m_strl5471Output.Add ("  5471: <PROCEED OFF>");
                                    Console.CursorSize = 100;
                                }
                            }
                        }

                        if ((yControlCode & k5471_SIO_KBD_Request_Key_Interrupts_Enable_Disable) > 0)
                        {
                            // Request key interrupts; 1 = enable, 0 = disable
                            if ((m_i5471StatusFlags & STAT_Kbd5471_Request_Key_Interrupt_Enabled) == 0)
                            {
                                m_i5471StatusFlags |= STAT_Kbd5471_Request_Key_Interrupt_Enabled;
                                m_bKeyboardInputEnabled = true;
                                if (m_bShowKeyboardIndicatorStatus)
                                {
                                    m_strl5471Output.Add ("  5471: <REQUEST PENDING ON>");
                                }
                            }
                        }
                        else
                        {
                            if ((m_i5471StatusFlags & STAT_Kbd5471_Request_Key_Interrupt_Enabled) > 0)
                            {
                                m_i5471StatusFlags &= ~STAT_Kbd5471_Request_Key_Interrupt_Enabled;
                                m_bKeyboardInputEnabled = false;
                                if (m_bShowKeyboardIndicatorStatus)
                                {
                                    m_strl5471Output.Add ("  5471: <REQUEST PENDING OFF>");
                                }
                            }
                        }

                        if ((yControlCode & k5471_SIO_KBD_Other_Key_Interrupts_Enable_Disable) > 0)
                        {
                            // Other key interrupts; 1 = enable, 0 = disable
                            m_i5471StatusFlags |= STAT_Kbd5471_Other_Interrupts_Enabled;
                        }
                        else
                        {
                            m_i5471StatusFlags &= ~STAT_Kbd5471_Other_Interrupts_Enabled;
                        }

                        if ((yControlCode & k5471_SIO_KBD_Reset_Request_Key_or_Other_Key_Interrupts) > 0)
                        {
                            // Reset request key or other key interrupts
                            m_i5471StatusFlags &= ~(STAT_Kbd5471_End_Or_Cancel_Interrupt_Pending |
                                                    STAT_Kbd5471_Return_Or_Data_Key_Interrupt_Pending |
                                                    STAT_Kbd5471_Key_Interrupt_Pending);
                            m_iIL = IL_Main; // Reset interrupt and return main program level
                        }
                    }
                }
                else if (m_eKeyboard == EKeyboard.KEY_5475)
                {
                    //Reset5475Sns001 ();
                    //Reset5475Sns010 ();
                    //Reset5475Sns011 ();
                    Reset5475StatusFlags ();

                    m_b5475_Keyboard_Program_Numeric_Mode = ((yControlCode & k5475_SIO_0_Program_Numeric_Mode) > 0);
                    m_b5475_Keyboard_Program_Lower_Shift  = ((yControlCode & k5475_SIO_1_Program_Lower_Shift) > 0);
                    m_b5475_Error_Indicator_On            = ((yControlCode & k5475_SIO_2_Set_Error_Indicator) > 0);

                    if ((yControlCode & k5475_SIO_4_Restore_Data_Key) > 0)
                    {
                        m_b5475_Data_Key_Latched = false;
                        m_bKeyboardInputEnabled  = true;
                    }

                    if ((yControlCode & k5475_SIO_5_Unlock_Data_Key) > 0)
                    {
                        m_b5475_Keys_Unlocked   = true;
                        m_bKeyboardInputEnabled = true;
                        m_bDataKeysEnabled      = true;
                    }

                    if ((yControlCode & k5475_SIO_6_Enable_Interrupt) > 0)
                    {
                        m_b5475_SNS_3L_Keyboard_Interrupts_Enabled  = true;
                        m_bKeyboardInputEnabled = true;
                    }
                    else
                    {
                        m_b5475_SNS_3L_Keyboard_Interrupts_Enabled  = false;
                        m_bKeyboardInputEnabled = false;
                    }

                    if ((yControlCode & k5475_SIO_7_Reset_Interrupt) > 0)
                    {
                        m_e5475Interrupt = E5475Interrupt.E5475_Interrupt_None;
                        if ((yControlCode & k5475_SIO_6_Enable_Interrupt) > 0)
                        {
                            m_iIL = IL_Main;
                        }
                    }

                    Show5475Status ();
                }
            }
            else
            {
                UnsupportedDevice ();
            }
        }
        #endregion

        private char ConvertToHexChar (byte yInput)
        {
            char cReturn = ' ';

            if (yInput >= 0x00 && yInput <= 0x09)
            {
                cReturn = (char)('0' + (char)yInput);
            }
            else if (yInput >= 0x0a && yInput <= 0x0f)
            {
                cReturn = (char)('A' + (char)(yInput - 10));
            }

            return cReturn;
        }

        public void WriteOutputStringList (List<string> strlInput, EOutputTarget eOutputTarget = EOutputTarget.OUTPUT_Unspecified,
                                           int iSpacing = 2, bool bClearAfter = false, bool bWriteLogOutputLine = false)
        {
            foreach (string str in strlInput)
            {
                WriteOutput (str, eOutputTarget, bWriteLogOutputLine);
            }

            for (int iCount = 0; iCount < iSpacing; ++iCount)
            {
                WriteOutput ("", eOutputTarget, bWriteLogOutputLine);
            }

            if (bClearAfter)
                strlInput.Clear ();
        }

        public void WriteOutput (string strOutput, EOutputTarget eOutputTarget = EOutputTarget.OUTPUT_Unspecified,
                                 bool bWriteLogOutputLine = false)
        {
            if (eOutputTarget == EOutputTarget.OUTPUT_Unspecified)
            {
                eOutputTarget = m_eOutputTarget;
            }

            if (!m_bAsyncRun)
            {
                Console.WriteLine (strOutput);
            }
            else if (eOutputTarget == EOutputTarget.OUTPUT_Console)
            {
                Console.WriteLine (strOutput);
            }
            else if (eOutputTarget == EOutputTarget.OUTPUT_5471Printer)
            {
                OnNew5471String?.Invoke (this, new New5471StringEventArgs (strOutput + Environment.NewLine));
            }
            else if (eOutputTarget == EOutputTarget.OUTPUT_DasmPanel)
            {
                OnNewDASMString?.Invoke (this, new NewDASMStringEventArgs (strOutput + Environment.NewLine));
                //OnNewPrinterString?.Invoke (this, new NewPrinterStringEventArgs (strOutput + Environment.NewLine));
            }
            else if (eOutputTarget == EOutputTarget.OUTPUT_TracePanel)
            {
                OnNewTraceString?.Invoke (this, new NewTraceStringEventArgs (strOutput + Environment.NewLine));
            }
            else if (eOutputTarget == EOutputTarget.OUTPUT_PrinterPanel)
            {
                if (m_objEmulatorState.IsFreeRun ())
                {
                    // Queue up lines, filtering out trailing blank lines, then print out at the end of program execution
                    m_objPrintQueue.Enqueue (strOutput);
                }
                else
                {
                    OnNewPrinterString?.Invoke (this, new NewPrinterStringEventArgs (strOutput + Environment.NewLine));
                }
            }

            if (bWriteLogOutputLine)
            {
                WriteLogOutputLine (strOutput);
            }
        }

        #region Diagnostic Methods
        // Test data
        //byte[] yaObjectCode1 = { 0x04, 0x01, 0x10, 0x21, 0x30, 0x41, // ZAZ -6
        //                         0x16, 0x02, 0x11, 0x22, 0x31,       // AZ  -5
        //                         0x27, 0x03, 0x12, 0x23, 0x32,       // SZ  -5
        //                         0x48, 0x04, 0x13, 0x24, 0x33,       // MVX -5
        //                         0x5A, 0x05, 0x14, 0x25,             // ED  -4
        //                         0x6B, 0x06, 0x15, 0x26,             // ITC -4
        //                         0x8C, 0x07, 0x16, 0x27, 0x34,       // MVC -5
        //                         0x9D, 0x07, 0x17, 0x28,             // CLC -4
        //                         0xAE, 0x08, 0x18, 0x29,             // ALC -4
        //                         0x30, 0x0A, 0x19, 0x2A,             // SNS -4
        //                         0xC0, 0x87, 0x19, 0x2A,             // BC  -4
        //                         0xD0, 0x87, 0x2A,                   // BC  -3
        //                         0xE0, 0x87, 0x2A,                   // BC  -3
        //                         0x31, 0x0B, 0x1A, 0x2A,             // LIO -4
        //                         0x71, 0x0B, 0x1A,                   // LIO -3
        //                         0xB4, 0x0C, 0x1B,                   // ST  -3
        //                         0xF2, 0x87, 0x1C };                 // JC  -3

        private void DisplayStatus ()
        {
            bool bBitsOn = false;
            StringBuilder strbldStatusFlags = new StringBuilder ("            Drive 1 Status Bits:");

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte0Bit0NoOp) != 0)
            {
                strbldStatusFlags.Append (" NoOp");
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte0Bit5NoRecordFound) != 0)
            {
                strbldStatusFlags.Append (" NoRec"); // NoRecordFound
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte0Bit7SeekCheck) != 0)
            {
                strbldStatusFlags.Append (" SkChk"); // SeekCheck
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte1Bit0ScanEqualHit) != 0)
            {
                strbldStatusFlags.Append (" SEqHit"); // ScanEqualHit
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte1Bit1CylinderZero) != 0)
            {
                strbldStatusFlags.Append (" Cyl0"); // CylinderZero
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte1Bit2EndOfCylinder) != 0)
            {
                strbldStatusFlags.Append (" EOCyl"); // EndOfCylinder
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte1Bit6StatusAddressA) != 0)
            {
                strbldStatusFlags.Append (" StatA"); // StatusAddressA
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte1Bit7StatusAddressB) != 0)
            {
                strbldStatusFlags.Append (" StatB"); // StatusAddressB
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte2Bit0Unsafe) != 0)
            {
                strbldStatusFlags.Append (" Unsafe");
                bBitsOn = true;
            }

            if (!bBitsOn)
            {
                strbldStatusFlags.Append (" <none>");
            }

            WriteLogOutputLine (strbldStatusFlags.ToString ());

            bBitsOn = false;
            strbldStatusFlags = new StringBuilder ("            Drive 2 Status Bits:");

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte0Bit0NoOp) != 0)
            {
                strbldStatusFlags.Append (" NoOp");
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte0Bit5NoRecordFound) != 0)
            {
                strbldStatusFlags.Append (" NoRec"); // NoRecordFound
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte0Bit7SeekCheck) != 0)
            {
                strbldStatusFlags.Append (" SkChk"); // SeekCheck
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte1Bit0ScanEqualHit) != 0)
            {
                strbldStatusFlags.Append (" SEqHit"); // ScanEqualHit
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte1Bit1CylinderZero) != 0)
            {
                strbldStatusFlags.Append (" Cyl0"); // CylinderZero
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte1Bit2EndOfCylinder) != 0)
            {
                strbldStatusFlags.Append (" EOCyl"); // EndOfCylinder
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte1Bit6StatusAddressA) != 0)
            {
                strbldStatusFlags.Append (" StatA"); // StatusAddressA
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte1Bit7StatusAddressB) != 0)
            {
                strbldStatusFlags.Append (" StatB"); // StatusAddressB
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte2Bit0Unsafe) != 0)
            {
                strbldStatusFlags.Append (" Unsafe");
                bBitsOn = true;
            }

            if (!bBitsOn)
            {
                strbldStatusFlags.Append (" <none>");
            }

            WriteLogOutputLine (strbldStatusFlags.ToString ());
        }

        public void Show5475Status ()
        {
            if (!m_bEnableShow5475Status)
            {
                return;
            }
            //             1         2         3         4         5         6         7         8
            //   012345678901234567890123456789012345678901234567890123456789012345678901234567890
            // 00   ASCII       LIO Program 1 Indicator          Read Key Pressed              +
            // 01  --    --     LIO Program 2 Indicator          Skip Key Pressed              +
            // 02 |  |  |  |    Error Indicator On               Dup Key Pressed               +
            // 03  --    --     Program Switch On                Field Erase Key Pressed       +
            // 04 |  |  |  |    Program One Selected             Record Erase Switch Operated  +
            // 05  --    --     Any Data Key                     Error Reset Key Pressed       +
            // 06   EBCDIC      Data Key Latched                 Program 1 Key Pressed         +
            // 07  --    --     Keys Unlocked                    Program 2 Key Pressed         +
            // 08 |  |  |  |    Keyboard Interrupts Enabled      Any Function Key Pressed      +
            // 09  --    --     Keyboard Program Numeric Mode    Release Key Pressed           +
            // 10 |  |  |  |    Keyboard Program Lower Shift     Auto Record Release Switch On +
            // 11  --    --     Lower Shift Key Pressed          Invalid Character Detected    +
            // 12               <interrupt type>                                               +

            //Console.WriteLine ("          1         2         3         4         5         6         7         8");
            //Console.WriteLine ("012345678901234567890123456789012345678901234567890123456789012345678901234567890");
            byte yTopDisplay    = m_bMultiPunch ? m_yMultiPunchCharacter : m_y5475Keystroke,
                 yBottomDisplay = m_y5475KeyboardInput;
            byte[] ya5475Codes  = { 0xEE, 0x24, 0xBA, 0xB6, 0x74, 0xD6, 0xDE, 0xA4,
                                    0xFE, 0xF6, 0xFC, 0x5E, 0xCA, 0x3E, 0xDA, 0xD8 };
            Console.CursorVisible = false;
            // Write 7-segment displays & header lines
            if (m_bMultiPunch)
            {
                Console.SetCursorPosition (0, 0);
                Console.Write ("MultiPunch");
            }
            else
            {
                Console.SetCursorPosition (0, 0);
                Console.Write ("   ASCII   +");
                WriteEndOfLine (11, 0);
            }

            int iLine = 1;
            List<string> strl5475Display = Get5475Display (ya5475Codes[(yTopDisplay >> 4)],
                                                           ya5475Codes[(yTopDisplay & 0x000F)]);
            foreach (string str in strl5475Display)
            {
                Console.SetCursorPosition (0, iLine);
                Console.Write (str + "+");
                // Write an invisible character since Write optimizes by truncating trailing spaces
                Console.SetCursorPosition (8, iLine++);
                ConsoleColor ccForeground = Console.ForegroundColor;
                Console.ForegroundColor = Console.BackgroundColor;
                Console.Write ('+');
                Console.ForegroundColor = ccForeground;
            }

            Console.SetCursorPosition (3, 6);
            Console.Write ("EBCDIC");
            iLine = 7;
            strl5475Display = Get5475Display (ya5475Codes[(yBottomDisplay >> 4)],
                                              ya5475Codes[(yBottomDisplay & 0x000F)]);
            foreach (string str in strl5475Display)
            {
                Console.SetCursorPosition (0, iLine);
                Console.Write (str + "+");
                // Write an invisible character since Write optimizes by truncating trailing spaces
                Console.SetCursorPosition (8, iLine++);
                ConsoleColor ccForeground = Console.ForegroundColor;
                Console.ForegroundColor = Console.BackgroundColor;
                Console.Write ('+');
                Console.ForegroundColor = ccForeground;
            }

            iLine = 0;
            Console.SetCursorPosition (15, iLine);
            Console.Write ((m_b5475_LIO_Program_1_Indicator ? "LIO Program 1 Indicator        " : "                                 ") +          
                           (m_b5475_SNS_2H_Read_Key_Pressed ? "Read Key Pressed              +"   : "                              +"));
            WriteEndOfLine (78, iLine++);

            Console.SetCursorPosition (15, iLine);
            Console.Write ((m_b5475_LIO_Program_2_Indicator ? "LIO Program 2 Indicator        " : "                                 ") +
                           (m_b5475_SNS_2L_Skip_Key_Pressed ? "Skip Key Pressed              +"   : "                              +"));
            WriteEndOfLine (78, iLine++);

            Console.SetCursorPosition (15, iLine);
            Console.Write ((m_b5475_Error_Indicator_On     ? "Error Indicator On             " : "                                 ") +
                           (m_b5475_SNS_2L_Dup_Key_Pressed ? "Dup Key Pressed               +"   : "                              +"));
            WriteEndOfLine (78, iLine++);

            Console.SetCursorPosition (15, iLine);
            Console.Write ((m_b5475_SNS_2L_Program_Switch_On       ? "Program Switch On              " : "Program Switch Off               ") +
                           (m_b5475_SNS_2H_Field_Erase_Key_Pressed ? "Field Erase Key Pressed       +"   : "                              +"));
            WriteEndOfLine (78, iLine++);

            Console.SetCursorPosition (15, iLine);
            Console.Write ((m_b5475_Program_One_Selected                ? "Program One Selected           " : "                                 ") +
                           (m_b5475_SNS_2L_Record_Erase_Switch_Operated ? "Record Erase Switch Operated  +"   : "                              +"));
            WriteEndOfLine (78, iLine++);

            Console.SetCursorPosition (15, iLine);
            Console.Write ((m_b5475_SNS_3L_Any_Data_Key            ? "Any Data Key                   " : "                                 ") +
                           (m_b5475_SNS_2H_Error_Reset_Key_Pressed ? "Error Reset Key Pressed       +"   : "                              +"));
            WriteEndOfLine (78, iLine++);

            Console.SetCursorPosition (15, iLine);
            Console.Write ((m_b5475_Data_Key_Latched             ? "Data Key Latched               " : "                                 ") +
                           (m_b5475_SNS_2H_Program_1_Key_Pressed ? "Program 1 Key Pressed         +"   : "                              +"));
            WriteEndOfLine (78, iLine++);

            Console.SetCursorPosition (15, iLine);
            Console.Write ((m_b5475_Keys_Unlocked                ? "Keys Unlocked                  " : "                                 ") +
                           (m_b5475_SNS_2H_Program_2_Key_Pressed ? "Program 2 Key Pressed         +"   : "                              +"));
            WriteEndOfLine (78, iLine++);

            Console.SetCursorPosition (15, iLine);
            Console.Write ((m_b5475_SNS_3L_Keyboard_Interrupts_Enabled ? "Keyboard Interrupts Enabled    " : "                                 ") +
                           (m_b5475_SNS_3L_Any_Function_Key_Pressed    ? "Any Function Key Pressed      +"   : "                              +"));
            WriteEndOfLine (78, iLine++);

            Console.SetCursorPosition (15, iLine);
            Console.Write ((m_b5475_Keyboard_Program_Numeric_Mode ? "Keyboard Program Numeric Mode  " : "                                 ") +
                           (m_b5475_SNS_2H_Release_Key_Pressed    ? "Release Key Pressed           +"   : "                              +"));
            WriteEndOfLine (78, iLine++);

            Console.SetCursorPosition (15, iLine);
            Console.Write ((m_b5475_Keyboard_Program_Lower_Shift         ? "Keyboard Program Lower Shift   " : "                                 ") +
                           (m_b5475_SNS_2L_Auto_Record_Release_Switch_On ? "Auto Record Release Switch On +"     : "Auto Record Release Switch Off+"));
            WriteEndOfLine (78, iLine++);

            Console.SetCursorPosition (15, iLine);
            Console.Write ((m_b5475_SNS_1L_Lower_Shift_Key_Pressed    ? "Lower Shift Key Pressed        " : "                                 ") +
                           (m_b5475_SNS_1L_Invalid_Character_Detected ? "Invalid Character Detected    +"   : "                              +"));
            WriteEndOfLine (78, iLine++);

            Console.SetCursorPosition (15, iLine);
            Console.Write ((m_e5475Interrupt == E5475Interrupt.E5475_Interrupt_None         ? "Keyboard Program Lower Shift     " :
                            m_e5475Interrupt == E5475Interrupt.E5475_Interrupt_Data_Key     ? "Data Key Interrupt               " :
                            m_e5475Interrupt == E5475Interrupt.E5475_Interrupt_Function_Key ? "Function Key Interrupt           " :
                            m_e5475Interrupt == E5475Interrupt.E5475_Interrupt_Multi_Punch  ? "Multi-Punch Interrupt            " :
                                                                                              "<undefined value>                ") +
                           (m_bKeyboardInputEnabled                                         ? "Keyboard Input Enabled        +" : "                              +"));
            WriteEndOfLine (78, iLine);
        }

        protected void WriteEndOfLine (int iLeft, int iTop)
        {
            Console.SetCursorPosition (iLeft, iTop);
            // Write an invisible character since Write optimizes by truncating trailing spaces
            ConsoleColor ccForeground = Console.ForegroundColor;
            Console.ForegroundColor = Console.BackgroundColor;
            Console.Write ('+');
            Console.ForegroundColor = ccForeground;
        }

        protected void WaveRedFlag (string strMessage)
        {
            if (!m_bAsyncRun &&
                m_bShowRedFlag)
            {
                int iLenghth = strMessage.Length + 12;
                string strAsteriskFrame = new string ('*', iLenghth),
                       strSpaceFiller   = new string (' ', iLenghth - 6);

                WriteLogOutputLine ("");
                WriteLogOutputLine (strAsteriskFrame);
                WriteLogOutputLine (strAsteriskFrame);
                WriteLogOutputLine ("***" + strSpaceFiller + "***");
                WriteLogOutputLine ("***   " + strMessage + "   ***");
                WriteLogOutputLine ("***" + strSpaceFiller + "***");
                WriteLogOutputLine (strAsteriskFrame);
                WriteLogOutputLine (strAsteriskFrame);
                WriteLogOutputLine ("");
            }
        }
        #endregion

        #region Instruction support methods
        protected byte[] FetchInstruction ()
        {
            int iInstructionLength = 0;

            // Fetch op code
            byte yOpCode = m_yaMainMemory[m_iaIAR[m_iIL]];
            //byte yOpCode = yaObjectCode1[m_iIAR[m_iIL]];

            // Determine instruction length from instruction format
            if ((yOpCode & 0xF0) == 0xF0)
            {
                // Command format: 3 bytes
                iInstructionLength = 3;
            }
            else if ((yOpCode & 0x30) == 0x30)
            {
                // Y format, either 3 or 4 bytes
                if ((yOpCode & 0xC0) == 0)
                {
                    iInstructionLength = 4; // No index registers: 4 bytes
                }
                else
                {
                    iInstructionLength = 3; // One index registers: 3 bytes
                }
            }
            else if ((yOpCode & 0xC0) == 0xC0)
            {
                // Z format, either 3 or 4 bytes
                if ((yOpCode & 0x30) == 0)
                {
                    iInstructionLength = 4; // No index registers: 4 bytes
                }
                else
                {
                    iInstructionLength = 3; // One index registers: 3 bytes
                }
            }
            else // X format, 4 to 6 bytes
            {
                bool bXR1 = ((yOpCode & 0xC0) > 0);
                bool bXR2 = ((yOpCode & 0x30) > 0);

                iInstructionLength = 6; // If no index registers are used

                if (bXR1)
                {
                    iInstructionLength--; // First operand is indexed
                }

                if (bXR2)
                {
                    iInstructionLength--; // Second operand is indexed
                }
            }

            // Create buffer for instruction
            byte[] yaInstruction = new byte[iInstructionLength];

            // Fetch instruction
            for (int iIdx = 0; iIdx < iInstructionLength; iIdx++)
            {
                yaInstruction[iIdx] = m_yaMainMemory[m_iaIAR[m_iIL] + iIdx];
                //yaInstruction[iIdx] = yaObjectCode1[m_iIAR[m_iIL] + iIdx];
            }

            // Increment IAR by instruction length
            m_iaIAR[m_iIL] += iInstructionLength;
            //if (m_objEmulatorState.IsSingleStep () && OnNewIAR != null) { OnNewIAR (this, new NewIAREventArgs (m_iaIAR[m_iIL], m_iIL)); }

            return yaInstruction;
        }

        protected int GetOneOperandAddress (byte[] yaInstruction)
        {
            byte yOpCode = yaInstruction[0];
            int iOperandAddress = 0;

            if ((yOpCode & 0x30) == 0x30)
            {
                // Y format, either 3 or 4 bytes
                if ((yOpCode & 0xC0) == 0)
                {
                    // No index registers: 4 bytes
                    iOperandAddress = yaInstruction[2];
                    iOperandAddress *= 0x0100;
                    iOperandAddress += yaInstruction[3];
                }
                else
                {
                    if ((yOpCode & 0xC0) == 0x40)
                    {
                        // Use index register 1
                        iOperandAddress = m_iXR1;
                        iOperandAddress += yaInstruction[2];
                        iOperandAddress &= 0x0FFFF;
                    }
                    else if ((yOpCode & 0xC0) == 0x80)
                    {
                        // Use index register 2
                        iOperandAddress = m_iXR2;
                        iOperandAddress += yaInstruction[2];
                        iOperandAddress &= 0x0FFFF;
                    }
                    else
                    {
                        // This code block should NEVER be entered
                        WaveRedFlag ("Undefined index register - Y format");
                    }
                }
            }
            else if ((yOpCode & 0xC0) == 0xC0)
            {
                // Z format, either 3 or 4 bytes
                if ((yOpCode & 0x30) == 0)
                {
                    // No index registers: 4 bytes
                    iOperandAddress = yaInstruction[2];
                    iOperandAddress *= 0x0100;
                    iOperandAddress += yaInstruction[3];
                }
                else
                {
                    if ((yOpCode & 0x30) == 0x10)
                    {
                        // Use index register 1
                        iOperandAddress = m_iXR1;
                        iOperandAddress += yaInstruction[2];
                        iOperandAddress &= 0x0FFFF;
                    }
                    else if ((yOpCode & 0x30) == 0x20)
                    {
                        // Use index register 2
                        iOperandAddress = m_iXR2;
                        iOperandAddress += yaInstruction[2];
                        iOperandAddress &= 0x0FFFF;
                    }
                    else
                    {
                        // This code block should NEVER be entered
                        WaveRedFlag ("Undefined index register - Z format");
                    }
                }
            }

            if (iOperandAddress > 0xFFFF)
            {
                iOperandAddress = 0xFFFF;
            }
            
            return iOperandAddress;
        }

        protected CTwoOperandAddress GetTwoOperandAddress (byte[] yaInstruction)
        {
            byte yOpCode = yaInstruction[0];
            int iOperandOneAddress = 0;
            int iOperandTwoAddress = 0;

            byte yOperandOneXR = (byte)(yaInstruction[0] & 0xC0);
            byte yOperandTwoXR = (byte)(yaInstruction[0] & 0x30);
            int iOperandTwoAddressOffset = (yOperandOneXR == 0) ? 4 : 3;

            // Computer operand 1 address
            if (yOperandOneXR == 0)
            {
                // No index register
                iOperandOneAddress = yaInstruction[2];
                iOperandOneAddress *= 0x0100;
                iOperandOneAddress += yaInstruction[3];
            }
            else if (yOperandOneXR == 0x40)
            {
                // Use index register 1
                iOperandOneAddress = m_iXR1;
                iOperandOneAddress += yaInstruction[2];
            }
            else if (yOperandOneXR == 0x80)
            {
                // Use index register 2
                iOperandOneAddress = m_iXR2;
                iOperandOneAddress += yaInstruction[2];
            }
            else
            {
                // This code block should NEVER be entered
                WaveRedFlag ("Undefined operand 1 index register - X format");
            }

            // Computer operand 2 address
            if (yOperandTwoXR == 0)
            {
                // No index register
                iOperandTwoAddress = yaInstruction[iOperandTwoAddressOffset];
                iOperandTwoAddress *= 0x0100;
                iOperandTwoAddress += yaInstruction[iOperandTwoAddressOffset + 1];
            }
            else if (yOperandTwoXR == 0x10)
            {
                // Use index register 1
                iOperandTwoAddress = m_iXR1;
                iOperandTwoAddress += yaInstruction[iOperandTwoAddressOffset];
            }
            else if (yOperandTwoXR == 0x20)
            {
                // Use index register 2
                iOperandTwoAddress = m_iXR2;
                iOperandTwoAddress += yaInstruction[iOperandTwoAddressOffset];
            }
            else
            {
                // This code block should NEVER be entered
                WaveRedFlag ("Undefined operand 2 index register - X format");
            }

            if (iOperandOneAddress > 0xFFFF)
            {
                iOperandOneAddress = 0xFFFF;
            }

            if (iOperandTwoAddress > 0xFFFF)
            {
                iOperandTwoAddress = 0xFFFF;
            }

            return new CTwoOperandAddress (iOperandOneAddress & 0xFFFF, iOperandTwoAddress & 0xFFFF);
        }

        protected int CompareZoned (int iOperandOneAddress, int iOperandOneLength, int iOperandTwoAddress, int iOperandTwoLength, bool bIgnoreSign)
        {
            int iOperandOnePointer = iOperandOneAddress - iOperandOneLength + 1,
                iOperandTwoPointer = iOperandTwoAddress - iOperandTwoLength + 1;

            if (!bIgnoreSign)
            {
                bool bOperandOneNegative = ((m_yaMainMemory[iOperandOneAddress] & 0xF0) == (byte)0xD0),
                     bOperandTwoNegative = ((m_yaMainMemory[iOperandTwoAddress] & 0xF0) == (byte)0xD0);

                if (bOperandOneNegative != bOperandTwoNegative)
                {
                    return (bOperandOneNegative ? -1 : 1);
                }
            }

            // Signs match or are ignored; proceed with the digit-by-digit comparison
            while (iOperandOnePointer <= iOperandOneAddress)
            {
                if ((iOperandOneAddress - iOperandOnePointer) > (iOperandTwoAddress - iOperandTwoPointer))
                {
                    if ((m_yaMainMemory[iOperandOnePointer] & (byte)0x0F) > 0x00)
                    {
                        return 1; // Operand 1 is greater because it has a significant digit before the leftmost digit in operand 2
                    }
                    iOperandOnePointer++;
                }
                else
                {
                    byte yOperandOneDigit = (byte)(m_yaMainMemory[iOperandOnePointer] & (byte)0x0F),
                         yOperandTwoDigit = (byte)(m_yaMainMemory[iOperandTwoPointer] & (byte)0x0F);

                    if (yOperandOneDigit > yOperandTwoDigit)
                    {
                        return 1; // Operand 1 is greater than operand 2
                    }
                    else if (yOperandOneDigit < yOperandTwoDigit)
                    {
                        return -1; // Operand 1 is less than operand 2
                    }

                    iOperandOnePointer++;
                    iOperandTwoPointer++;
                }
            }

            return 0; // Equal value
        }

        private void ExecuteDecimalArithmetic (byte[] yaInstruction, bool bAddition)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];
            int iOperandTwoLength = (yQByte & 0x0F) + 1,
                iOperandOneLength = (yQByte >> 4) + iOperandTwoLength;

            if (ZonedAddressesValid (objTOA, yQByte))
            {
                int iOperandOneAddress = objTOA.OperandOneAddress,
                    iOperandTwoAddress = objTOA.OperandTwoAddress;

                // Check for invalid operand overlap
                if (iOperandOneAddress < iOperandTwoAddress &&
                    iOperandOneAddress > iOperandTwoAddress - iOperandTwoLength)
                {
                    InvalidQByte ();
                    return;
                }

                bool bOperandOneNegative =              (m_yaMainMemory[iOperandOneAddress] & 0xF0) == (byte)0xD0,
                     bOperandTwoNegative = bAddition ? ((m_yaMainMemory[iOperandTwoAddress] & 0xF0) == (byte)0xD0) :
                                                       ((m_yaMainMemory[iOperandTwoAddress] & 0xF0) != (byte)0xD0),
                     bSubtract           = bOperandOneNegative != bOperandTwoNegative,
                     bSwapOperands       = false,
                     bCarry              = false,
                     bNextCarry          = false,
                     bResultZero         = true,
                     bResultNegative     = bOperandOneNegative && bOperandTwoNegative;

                if (bSubtract)
                {
                    int iCompare = CompareZoned (iOperandOneAddress, iOperandOneLength, iOperandTwoAddress, iOperandTwoLength, true);
                    bSwapOperands = iCompare < 0;
                    if ((iCompare > 0 && bOperandOneNegative) ||
                        (iCompare < 0 && bOperandTwoNegative))
                    {
                        bResultNegative = true;
                    }
                }

                // Do the math
                for (int iOffset = 0; iOffset < iOperandOneLength; iOffset++)
                {
                    byte yOperandOneDigit = (byte)((m_yaMainMemory[iOperandOneAddress - iOffset] & 0x0F) % 10),
                         yOperandTwoDigit = (byte)((iOffset < iOperandTwoLength) ?
                                            ((m_yaMainMemory[iOperandTwoAddress - iOffset] & 0x0F) % 10) : 0x00);

                    if (bSubtract)
                    {
                        if (bSwapOperands)
                        {
                            byte yHold = yOperandOneDigit;
                            yOperandOneDigit = yOperandTwoDigit;
                            yOperandTwoDigit = yHold;
                        }

                        if (yOperandOneDigit < yOperandTwoDigit ||
                            (bCarry &&
                             yOperandOneDigit == yOperandTwoDigit))
                        {
                            yOperandOneDigit += (byte)10;
                            bNextCarry = true;
                        }

                        int iResult = yOperandOneDigit - yOperandTwoDigit;
                        if (bCarry)
                        {
                            iResult--;
                            bCarry = false;
                        }

                        bCarry = bNextCarry;
                        bNextCarry = false;

                        m_yaMainMemory[iOperandOneAddress - iOffset] = (byte)(iResult & 0xFF);
                    }
                    else
                    {
                        int iResult = yOperandOneDigit + yOperandTwoDigit;
                        if (bCarry)
                        {
                            iResult++;
                            bCarry = false;
                        }

                        if (iResult > 9)
                        {
                            iResult = iResult % 10;
                            bCarry = true;
                        }

                        m_yaMainMemory[iOperandOneAddress - iOffset] = (byte)(iResult & 0xFF);
                    }
                }

                // Set all the zones
                for (int iOffset = 0; iOffset < iOperandOneLength; iOffset++)
                {
                    m_yaMainMemory[iOperandOneAddress - iOffset] |= (byte)0xF0;
                    if ((m_yaMainMemory[iOperandOneAddress - iOffset] & (byte)0x0F) > 0)
                    {
                        bResultZero   = false;
                        m_iaARR[m_iIL] = iOperandOneAddress - iOffset;
                        if (m_objEmulatorState.IsSingleStep () && OnNewARR != null) { OnNewARR (this, new NewARREventArgs (m_iaARR[m_iIL], m_iIL)); }
                    }
                }

                // Set the sign zone
                if (bResultNegative)
                {
                    m_yaMainMemory[iOperandOneAddress] &= (byte)0xDF;
                    m_yaMainMemory[iOperandOneAddress] |= (byte)0xD0;
                }

                // Set the condition register
                if (bResultZero)
                {
                    m_aCR[m_iIL].SetEqual ();
                    m_iaARR[m_iIL] = iOperandOneAddress + 1; // No significant digits
                    if (m_objEmulatorState.IsSingleStep () && OnNewARR != null) { OnNewARR (this, new NewARREventArgs (m_iaARR[m_iIL], m_iIL)); }
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
                else if (bResultNegative)
                {
                    m_aCR[m_iIL].SetLow ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
                else
                {
                    m_aCR[m_iIL].SetHigh ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }

                if (bCarry)
                {
                    m_aCR[m_iIL].SetDecimalOverflow ();
                    if (m_objEmulatorState.IsSingleStep () && OnNewCR != null && m_iIL == IL_Main) { OnNewCR (this, new NewCREventArgs ((byte)m_aCR[IL_Main].Store ())); }
                }
            }
        }

        private int LoadInt (int iAddress)
        {
            return (m_yaMainMemory[iAddress - 1] << 8) + (m_yaMainMemory[iAddress]);
        }

        private void LoadRegister (int iRegister, int iTargetAddress)
        {
            if (AddressWraps (iTargetAddress, 2))
            {
                AddressWraparound ();
            }
            else
            {
                iRegister  = (int)m_yaMainMemory[iTargetAddress - 1] * 0x0100;
                iRegister += (int)m_yaMainMemory[iTargetAddress];
            }
        }

        private void StoreRegister (int iRegister, int iTargetAddress)
        {
            if (AddressWraps (iTargetAddress, 2))
            {
                AddressWraparound ();
            }
            else
            {
                m_yaMainMemory[iTargetAddress - 1] = (byte)(iRegister >> 8);
                m_yaMainMemory[iTargetAddress]     = (byte)(iRegister & 0xFF);
            }
        }

        private string LoadString (int iStartAddress, int iLength)
        {
            StringBuilder strbldrData = new StringBuilder (iLength);

            for (int iIdx = 0; iIdx < iLength; iIdx++)
            {
                strbldrData.Append ((char)m_yaMainMemory[iStartAddress + iIdx]);
            }

            return strbldrData.ToString ();
        }

        private void StoreString (string strData, int iStartAddress)
        {
            for (int iIdx = 0; iIdx < strData.Length; iIdx++)
            {
                m_yaMainMemory[iStartAddress + iIdx] = (byte)strData[iIdx];
            }
        }

        private void LoadDiskControlField ()
        {
            m_objDiskControlField.yF = m_yaMainMemory[m_iDiskControlAddressRegister];
            m_objDiskControlField.yC = m_yaMainMemory[m_iDiskControlAddressRegister + 1];
            m_objDiskControlField.yS = m_yaMainMemory[m_iDiskControlAddressRegister + 2];
            m_objDiskControlField.yN = m_yaMainMemory[m_iDiskControlAddressRegister + 3];
        }

        private void StoreDiskControlField ()
        {
            m_yaMainMemory[m_iDiskControlAddressRegister]     = m_objDiskControlField.yF;
            m_yaMainMemory[m_iDiskControlAddressRegister + 1] = m_objDiskControlField.yC;
            m_yaMainMemory[m_iDiskControlAddressRegister + 2] = m_objDiskControlField.yS;
            m_yaMainMemory[m_iDiskControlAddressRegister + 3] = m_objDiskControlField.yN;
        }

        private string LoadStringEBCDIC (int iStartAddress, int iLength)
        {
            StringBuilder strbldrData = new StringBuilder (iLength);

            for (int iIdx = 0; iIdx < iLength; iIdx++)
            {
                strbldrData.Append ((char)ConvertEBCDICtoASCII (m_yaMainMemory[iStartAddress + iIdx]));
            }

            return strbldrData.ToString ();
        }

        private void StoreStringEBCDIC (string strData, int iStartAddress)
        {
            for (int iIdx = 0; iIdx < strData.Length; iIdx++)
            {
                m_yaMainMemory[iStartAddress + iIdx] = ConvertASCIItoEBCDIC ((byte)strData[iIdx]);
            }
        }

        private string PrintEBCDIC ()
        {
            StringBuilder strbldrPrintEBCDIC = new StringBuilder (m_obj5203LinePrinter.i5203LineWidth);

            for (int iIdx = 0; iIdx < m_obj5203LinePrinter.i5203LineWidth; iIdx++)
            {
                //byte yEBCDIC = m_yaMainMemory[m_i5203DataAddressRegister + iIdx];
                //strbldrPrintEBCDIC.Append (yEBCDIC > 0x00 ? (char)yEBCDIC : ' ');
                //char cASCII  = (char)ConvertEBCDICtoASCII (m_yaMainMemory[m_i5203DataAddressRegister + iIdx]);
                char cASCII = (char)ConvertEBCDICtoASCII (m_yaMainMemory[m_obj5203LinePrinter.i5203PrintDAR + iIdx]);
                if (m_bAllowNonAsciiPrint ||
                    cASCII == ' '         ||
                    m_obj5203LinePrinter.IsInChainImage (cASCII))
                {
                    //m_yaMainMemory[m_i5203DataAddressRegister + iIdx] = 0x40;
                    m_yaMainMemory[m_obj5203LinePrinter.i5203PrintDAR + iIdx] = 0x40;
                    strbldrPrintEBCDIC.Append (cASCII);
                }
                else
                {
                    strbldrPrintEBCDIC.Append (IsAscii (cASCII) ? cASCII : ' ');
                    m_obj5203LinePrinter.SetUnprintableCharacter ();
                }
            }

            return strbldrPrintEBCDIC.ToString ();
        }

        public void LoadBinaryImage (byte[] yaProgramImage, int iStartAddress, int iLength)
        {
            if (iLength <= 0)
            {
                iLength = yaProgramImage.Length;
            }

            for (int iIdx = 0; iIdx < iLength; iIdx++)
            {
                m_yaMainMemory[iStartAddress + iIdx] = yaProgramImage[iIdx % yaProgramImage.Length];
            }
        }
        #endregion

        #region Debugger methods
        private string FormatInstructionLine (string strInstruction, byte[] yaInstruction, int iIAR)
        {
            //List<string> strlInstruction = DisassembleCodeImage (yaInstruction, 0, yaInstruction.Length - 1, m_iXR1, m_iXR2, iIAR);
            if ((yaInstruction[0] & 0xF0) == 0xF0) // Command instruction
            {
                int iColon = strInstruction.IndexOf (':');
                //Console.WriteLine (strInstruction);
                //Console.WriteLine (strInstruction.Substring (iColon + 2, 14));
                return string.Format ("{0:X4}: {1:S}", iIAR, strInstruction.Substring (iColon + 2, 14));
            }
            else if (((yaInstruction[0] & 0xC0) == 0xC0) || // One-operand instruction
                     ((yaInstruction[0] & 0x30) == 0x30))
            {
                int iColon = strInstruction.IndexOf (':');
                //Console.WriteLine (strInstruction);
                //Console.WriteLine (strInstruction.Substring (iColon + 2, 16));
                int iOperandAddress = GetOneOperandAddress (yaInstruction);
                return string.Format ("{0:X4}: {1:S}       {2:X2} {3:X2}",
                                      iIAR,
                                      strInstruction.Substring (iColon + 2, 16),
                                      iOperandAddress >> 8,
                                      iOperandAddress & 0x00FF);
            }
            else // Two-operand instruction
            {
                int iColon = strInstruction.IndexOf (':');
                //Console.WriteLine (strInstruction);
                //Console.WriteLine (strInstruction.Substring (iColon + 2, 21));
                CTwoOperandAddress obj2O = GetTwoOperandAddress (yaInstruction);
                return string.Format ("{0:X4}: {1:S}  {2:X2} {3:X2}  {4:X2} {5:X2}",
                                      iIAR,
                                      strInstruction.Substring (iColon + 2, 21),
                                      obj2O.OperandOneAddress >> 8,
                                      obj2O.OperandOneAddress & 0x00FF,
                                      obj2O.OperandTwoAddress >> 8,
                                      obj2O.OperandTwoAddress & 0x00FF);
            }
        }

        // Test case: both 7 bytes
        //            both 8 bytes
        //            both 9 bytes
        //            one 7 bytes, on 9 bytes
        //            both 9 bytes
        //            both 15 bytes
        //            both 16 bytes
        //            both 17 bytes, one on 0x0010 boundary, the other not
        //            both 65 bytes not on boundary
        private List<string> FormatDataContents (int iOp1Start, int iOp1End, int iOp2Start, int iOp2End)
        {
            List<string> strlDataContents = new List<string> ();
            if (iOp1Start > iOp1End ||
                iOp2Start > iOp2End)
                return strlDataContents;
            
            StringBuilder strbData = new StringBuilder ();
            int iLength1 = iOp1End - iOp1Start + 1,
                iLength2 = (iOp2Start >= 0 && iOp2End >= 0) ? iOp2End - iOp2Start + 1 : -1;

            // First case: both operands short enough to fit on a single line
            if (iLength1 <= 8 && iLength1 > 0 &&
                iLength2 <= 8)
            {
                // Shorter data strings
                StringBuilder strbDataString = new StringBuilder ();

                strbDataString.Append ("Op1: " + FormatShortDataString (iOp1Start, iOp1End));

                if (iOp2Start >= 0 && iOp2End >= 0)
                {
                    strbDataString.Append ("  Op2: " + FormatShortDataString (iOp2Start, iOp2End));
                }

                strlDataContents.Add (strbDataString.ToString ());

                return strlDataContents;
            }

            // Print operand 2
            if (iOp2Start >= 0 && iOp2End >= 0)
            {
                if (iLength2 > 16)
                {
                    // Second case: Wrapped lines
                    List<string> strlDataOp2 = FormatWrappedDataString (iOp2Start, iOp2End, "Op2: ");
                    AppendStringList (strlDataOp2, strlDataContents);
                }
                else
                {
                    // Third case: Single line
                    strlDataContents.Add ("Op2: " + FormatDataString (iOp2Start, iOp2End));
                }
            }

            // Print operand 1
            if (iOp1Start >= 0 && iOp1End >= 0)
            {
                if (iLength1 > 16)
                {
                    // Second case: Wrapped lines
                    List<string> strlDataOp1 = FormatWrappedDataString (iOp1Start, iOp1End, "Op1: ");
                    AppendStringList (strlDataOp1, strlDataContents);
                }
                else
                {
                    // Third case: Single line
                    strlDataContents.Add ("Op1: " + FormatDataString (iOp1Start, iOp1End));
                }
            }

            return strlDataContents;
        }

        // Op2: xxxx: 00 01 02 03  04 05 06 07  08 09 0A 0B  0C 0D 0E 0F <0123 4567 89AB CDEF> <0123 4567 89AB CDEF>
        //      xxxx: 10 11 12 13  14 15 16 17                           <0123 4567          > <0123 4567          >
        // Op1: xxxx: 00 01 02 03  04 05 06 07  08 09 0A 0B  0C 0D 0E 0F <0123 4567 89AB CDEF> <0123 4567 89AB CDEF>
        //      xxxx: 10 11 12 13  14 15 16 17                           <0123 4567          > <0123 4567          >
        // Op1: xxxx: 00 01 02 03  04 05 06 07  08 09 0A 0B  0C 0D 0E 0F <0123 4567 89AB CDEF> <0123 4567 89AB CDEF>
        //      xxxx: 10 11 12 13  14 15 16 17                           <0123 4567          > <0123 4567          >
        // Must wrap lines anyway, so begin on 16-byte boundary
        // Op1: xxxx: 00 01 02 03  04 05 06 07  08 09 0A 0B  0C 0D 0E 0F <0123 4567 89AB CDEF> <0123 4567 89AB CDEF>
        // Op1: xxxx:                 00 01 02  03 04 05 06  07 08 09 0A <      012 3456 789A> <      012 3456 789A>
        //      xxxx: 0B 0C 0D 0E  0F 10                                 <BCDE F0            > <BCDE F0            >
        private List<string> FormatWrappedDataString (int iStart, int iEnd, string strLabel)
        {
            if (iStart > iEnd)
                return null;
            
            List<string> strlData = new List<string> ();
            StringBuilder strbData = new StringBuilder ();

            // First line, which may have padding
            strlData.Add (strLabel + FormatDataString (iStart, iStart | 0x000F, true));

            // Remaining lines
            int iStartAddress = (iStart & 0xFFF0) + 0x0010;
            for (int iAddress = iStartAddress; iAddress <= iEnd; iAddress += 16)
            {
                strlData.Add (new string (' ', strLabel.Length) + FormatDataString (iAddress, iEnd));
            }

            return strlData;
        }

        //          1         2         3         4         5         6         7         8         9         0
        // 1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
        // xxxx: 00 01 02 03  04 05 06 07 <0123 4567> <0123 4567>
        // xxxx: 00 01 02 03 04 05 06 07 <01234567> <01234567F> xxxx: 00 01 02 03 04 05 06 07 <01234567> <01234567F>
        // Op1: xxxx: 00 01 02 03  04 05 06 07 <0123 4567> <0123 4567>  Op2: xxxx: 00 01 02 03  04 05 06 07 <0123 4567> <0123 4567>
        private string FormatShortDataString (int iStart, int iEnd)
        {
            if (iStart == -1 ||
                iEnd   == -1 ||
                iStart > iEnd)
                return "";
            int iLength = iEnd - iStart + 1;

            StringBuilder strbDataString = new StringBuilder ();

            // Address
            byte yAddressHi = (byte)((iStart >> 8) & 0x000000FF),
                 yAddressLo = (byte)(iStart & 0x000000FF);
            strbDataString.Append (ByteToHexPair (yAddressHi) + ByteToHexPair (yAddressLo) + ": ");

            // Hex string
            for (int iIdx = 0; iIdx < iLength; ++iIdx)
            {
                if (iIdx > 0 && iIdx % 4 == 0)
                {
                    strbDataString.Append (' ');
                }
                strbDataString.Append (ByteToHexPair (m_yaMainMemory[iStart + iIdx]) + ' ');
            }

            // Pad hex string
            if (strbDataString.Length < 31)
            {
                strbDataString.Append (new string (' ', 31 - strbDataString.Length));
            }
            strbDataString.Append (" <");

            // ASCII string
            for (int iIdx = 0; iIdx < iLength; ++iIdx)
            {
                if (iIdx > 0 && iIdx % 4 == 0)
                {
                    strbDataString.Append (' ');
                }

                char cData = (char)m_yaMainMemory[iStart + iIdx];
                if (cData >= (char)0x20 &&
                    cData <  (char)0x7F)
                {
                    strbDataString.Append (cData);
                }
                else
                {
                    strbDataString.Append ('.');
                }
            }

            // Pad ASCII string
            if (strbDataString.Length < 42)
            {
                strbDataString.Append (new string (' ', 42 - strbDataString.Length));
            }
            strbDataString.Append (">  <");

            // EBCDIC string
            for (int iIdx = 0; iIdx < iLength; ++iIdx)
            {
                if (iIdx > 0 && iIdx % 4 == 0)
                {
                    strbDataString.Append (' ');
                }

                char cData = (char)ConvertEBCDICtoASCII (m_yaMainMemory[iStart + iIdx]);
                if (cData >= (char)0x20 &&
                    cData <  (char)0x7F)
                {
                    strbDataString.Append (cData);
                }
                else
                {
                    strbDataString.Append ('.');
                }
            }

            // Pad EBCDIC string
            if (strbDataString.Length < 55)
            {
                strbDataString.Append (new string (' ', 55 - strbDataString.Length));
            }
            strbDataString.Append ('>');

            return strbDataString.ToString ();
        }

        //          1         2         3         4         5         6         7         8         9         0
        // 1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
        // xxxx: 00 01 02 03  04 05 06 07  08 09 0A 0B  0C 0D 0E 0F <0123 4567 89AB CDEF> <0123 4567 89AB CDEF>
        // xxxx: __ __ __ __  __ 05 06 07  08 09 0A 0B  0C 0D 0E 0F <____ _567 89AB CDEF> <____ _567 89AB CDEF>
        private string FormatDataString (int iStart, int iEnd)
        {
            return FormatDataString (iStart, iEnd, false);
        }

        private string FormatDataString (int iStart, int iEnd, bool bEndOnBoundary)
        {
            if (iStart > iEnd)
            {
                return null;
            }

            int iPadding = 0;
            if (bEndOnBoundary)
            {
                iEnd = ((iStart + 0x10) & 0xFFF0) - 1;
                iPadding = iStart - (iStart & 0xFFF0);
            }
            else if (iEnd > iStart + 15)
            {
                iEnd = iStart + 15;
            }

            int iLength = iEnd - iStart + 1 + iPadding;

            StringBuilder strbDataString = new StringBuilder ();

            // Address
            byte yAddressHi = (byte)((iStart >> 8) & 0x000000FF),
                 yAddressLo = (byte)(iStart & 0x000000FF);
            strbDataString.Append (ByteToHexPair (yAddressHi) + ByteToHexPair (yAddressLo) + ": ");

            // Hex string
            for (int iIdx = 0; iIdx < iLength; ++iIdx)
            {
                if (iIdx > 0 && iIdx % 4 == 0)
                {
                    strbDataString.Append (' ');
                }
                if (iIdx < iPadding)
                {
                    strbDataString.Append ("   ");
                }
                else if (bEndOnBoundary)
                {
                    strbDataString.Append (ByteToHexPair (m_yaMainMemory[(iStart & 0xFFF0) + iIdx]) + ' ');
                }
                else
                {
                    strbDataString.Append (ByteToHexPair (m_yaMainMemory[iStart + iIdx]) + ' ');
                }
            }

            // Pad hex string
            if (strbDataString.Length < 57)
            {
                strbDataString.Append (new string (' ', 57 - strbDataString.Length));
            }
            strbDataString.Append (" <");

            // ASCII string
            for (int iIdx = 0; iIdx < iLength; ++iIdx)
            {
                if (iIdx > 0 && iIdx % 4 == 0)
                {
                    strbDataString.Append (' ');
                }

                if (iIdx < iPadding)
                {
                    strbDataString.Append (' ');
                }
                else
                {
                    //char cData = (char)m_yaMainMemory[(bEndOnBoundary ? (iStart & 0xFFF0) : iStart) + iIdx];
                    int iTempStart = bEndOnBoundary ? (iStart & 0xFFF0) : iStart;
                    char cData = (char)m_yaMainMemory[iTempStart + iIdx];
                    if (cData >= (char)0x20 &&
                        cData <  (char)0x7F)
                    {
                        strbDataString.Append (cData);
                    }
                    else
                    {
                        strbDataString.Append ('.');
                    }
                }
            }

            // Pad ASCII string
            if (strbDataString.Length < 78)
            {
                strbDataString.Append (new string (' ', 78 - strbDataString.Length));
            }
            strbDataString.Append (">  <");

            // EBCDIC string
            for (int iIdx = 0; iIdx < iLength; ++iIdx)
            {
                if (iIdx > 0 && iIdx % 4 == 0)
                {
                    strbDataString.Append (' ');
                }

                if (iIdx < iPadding)
                {
                    strbDataString.Append (' ');
                }
                else
                {
                    //char cData = (char)ConvertEBCDICtoASCII (m_yaMainMemory[(bEndOnBoundary ? (iStart & 0xFFF0) : iStart) + iIdx]);
                    int iTempStart = bEndOnBoundary ? (iStart & 0xFFF0) : iStart;
                    char cData = (char)ConvertEBCDICtoASCII (m_yaMainMemory[iTempStart + iIdx]);
                    if (cData >= (char)0x20 &&
                        cData <  (char)0x7F)
                    {
                        strbDataString.Append (cData);
                    }
                    else
                    {
                        strbDataString.Append ('.');
                    }
                }
            }

            // Pad EBCDIC string
            if (strbDataString.Length < 101)
            {
                strbDataString.Append (new string (' ', 101 - strbDataString.Length));
            }
            strbDataString.Append ('>');

            //Console.WriteLine (strbDataString.ToString ());
            return strbDataString.ToString ();
        }

        private string GetCardImage (int iDAR, int iColumnCount)
        {
            StringBuilder strbBuffer = new StringBuilder ();

            for (int iIdx = 0; iIdx < iColumnCount; ++iIdx)
            {
                char cNewChar = ((char)ConvertEBCDICtoASCII (m_yaMainMemory[iDAR + iIdx]));
                //strbBuffer.Append (IsPrintable (cNewChar) ? cNewChar : ' ');
                strbBuffer.Append (cNewChar);
            }

            return strbBuffer.ToString ();
        }

        private List<string> DumpMFCUBuffer (int iDAR, bool bShow4Lines = false)
        {
            List<string> strlBuffer = new List<string> ();

            if (m_objEmulatorState.IsSingleStep ())
            {
                for (int iIdx = 0; iIdx < (bShow4Lines ? 128 : 96); iIdx += 16)
                {
                    strlBuffer.Add (FormatDataString (iDAR + iIdx, iDAR + iIdx + 15));
                }
            }
            if (!m_objEmulatorState.IsSingleStep () &&
                !m_bMFCUBufferRulerShown)
            {
                AppendStringList (MakeRulerLines (bShow4Lines ? 32 : 96, 0), strlBuffer);
                m_bMFCUBufferRulerShown = true;
            }

            StringBuilder strbBuffer = new StringBuilder ();
            if (bShow4Lines)
            {
                // Top tier
                for (int iIdx = 0; iIdx < 32; ++iIdx)
                {
                    strbBuffer.Append ((char)ConvertEBCDICtoASCII (m_yaMainMemory[iDAR + iIdx]));
                }
                strlBuffer.Add (strbBuffer.ToString ());
                strbBuffer.Clear ();

                // 2nd tier
                for (int iIdx = 32; iIdx < 64; ++iIdx)
                {
                    strbBuffer.Append ((char)ConvertEBCDICtoASCII (m_yaMainMemory[iDAR + iIdx]));
                }
                strlBuffer.Add (strbBuffer.ToString ());
                strbBuffer.Clear ();

                // 3rd tier
                for (int iIdx = 64; iIdx < 96; ++iIdx)
                {
                    strbBuffer.Append ((char)ConvertEBCDICtoASCII (m_yaMainMemory[iDAR + iIdx]));
                }
                strlBuffer.Add (strbBuffer.ToString ());
                strbBuffer.Clear ();

                // 4th tier
                for (int iIdx = 96; iIdx < 128; ++iIdx)
                {
                    strbBuffer.Append ((char)ConvertEBCDICtoASCII (m_yaMainMemory[iDAR + iIdx]));
                }
                strlBuffer.Add (strbBuffer.ToString ());
            }
            else
            {
                for (int iIdx = 0; iIdx < 96; ++iIdx)
                {
                    char cNewChar = ((char)ConvertEBCDICtoASCII (m_yaMainMemory[iDAR + iIdx]));
                    strbBuffer.Append (IsPrintable (cNewChar) ? cNewChar : ' ');
                }
            }
            strlBuffer.Add (strbBuffer.ToString ());

            return strlBuffer;
        }

        private List<string> ShowValues (byte[] yaInstruction)
        {
            List<string> strlValues = new List<string> ();
            
            return strlValues;
        }

        private string GetConditionFlags (CConditionRegister cr)
        {
            StringBuilder strbldrFlags = new StringBuilder (16);

            if (cr.IsEqual ())
            {
                strbldrFlags.Append ("EQ");
            }
            else if (cr.IsHigh ())
            {
                strbldrFlags.Append ("HI");
            }
            else
            {
                strbldrFlags.Append ("LO");
            }

            strbldrFlags.Append (cr.IsDecimalOverflow () ? " DO" : "   ");
            strbldrFlags.Append (cr.IsTestFalse ()       ? " TF" : "   ");
            strbldrFlags.Append (cr.IsBinaryOverflow ()  ? " BO" : "   ");

            return strbldrFlags.ToString ();
        }
        #endregion

        #region 7-Segment Display Support
        //  Halt Codes:
        //  0x04   ---
        //  0x08  |   |  0x02
        //  0x10   ---
        //  0x20  |   |  0x01
        //  0x40   ---

        //  5475 Codes
        //  0x80   ---
        //  0x40  |   |  0x20
        //  0x10   ---   
        //  0x08  |   |  0x04
        //  0x02   ---

        protected List<string> GetHaltDisplay (byte yLeftDisplay, byte yRightDisplay)
        {
            List<string> strlHaltDisplay = new List<string> (5);

            strlHaltDisplay.Add (string.Format (" {0:S1}    {1:S1} ",           (yLeftDisplay  & 0x04) == 0x04 ? "-" : " ",
                                                                                (yRightDisplay & 0x04) == 0x04 ? "-" : " "));

            strlHaltDisplay.Add (string.Format ("{0:S1} {1:S1}  {2:S1} {3:S1}", (yLeftDisplay  & 0x08) == 0x08 ? "|" : " ",
                                                                                (yLeftDisplay  & 0x02) == 0x02 ? "|" : " ",
                                                                                (yRightDisplay & 0x08) == 0x08 ? "|" : " ",
                                                                                (yRightDisplay & 0x02) == 0x02 ? "|" : " "));

            strlHaltDisplay.Add (string.Format (" {0:S1}    {1:S1} ",           (yLeftDisplay  & 0x10) == 0x10 ? "-" : " ",
                                                                                (yRightDisplay & 0x10) == 0x10 ? "-" : " "));

            strlHaltDisplay.Add (string.Format ("{0:S1} {1:S1}  {2:S1} {3:S1}", (yLeftDisplay  & 0x20) == 0x20 ? "|" : " ",
                                                                                (yLeftDisplay  & 0x01) == 0x01 ? "|" : " ",
                                                                                (yRightDisplay & 0x20) == 0x20 ? "|" : " ",
                                                                                (yRightDisplay & 0x01) == 0x01 ? "|" : " "));

            strlHaltDisplay.Add (string.Format (" {0:S1}    {1:S1} ",           (yLeftDisplay  & 0x40) == 0x40 ? "-" : " ", 
                                                                                (yRightDisplay & 0x40) == 0x40 ? "-" : " "));

            return strlHaltDisplay;
        }

        protected List<string> Get5475Display (byte yLeftDisplay, byte yRightDisplay)
        {
            List<string> strl5475Display = new List<string> (5);

            strl5475Display.Add (string.Format (" {0:S1}    {1:S1} ",           (yLeftDisplay  & 0x80) == 0x80 ? "-" : " ",
                                                                                (yRightDisplay & 0x80) == 0x80 ? "-" : " "));

            strl5475Display.Add (string.Format ("{0:S1} {1:S1}  {2:S1} {3:S1}", (yLeftDisplay  & 0x40) == 0x40 ? "|" : " ",
                                                                                (yLeftDisplay  & 0x20) == 0x20 ? "|" : " ",
                                                                                (yRightDisplay & 0x40) == 0x40 ? "|" : " ",
                                                                                (yRightDisplay & 0x20) == 0x20 ? "|" : " "));

            strl5475Display.Add (string.Format (" {0:S1}    {1:S1} ",           (yLeftDisplay  & 0x10) == 0x10 ? "-" : " ",
                                                                                (yRightDisplay & 0x10) == 0x10 ? "-" : " "));

            strl5475Display.Add (string.Format ("{0:S1} {1:S1}  {2:S1} {3:S1}", (yLeftDisplay  & 0x08) == 0x08 ? "|" : " ",
                                                                                (yLeftDisplay  & 0x04) == 0x04 ? "|" : " ",
                                                                                (yRightDisplay & 0x08) == 0x08 ? "|" : " ",
                                                                                (yRightDisplay & 0x04) == 0x04 ? "|" : " "));

            strl5475Display.Add (string.Format (" {0:S1}    {1:S1} ",           (yLeftDisplay  & 0x02) == 0x02 ? "-" : " ",
                                                                                (yRightDisplay & 0x02) == 0x02 ? "-" : " "));

            return strl5475Display;
        }

        protected byte ConvertHaltCodeTo5745 (byte yHaltCode)
        {
            byte y5475Code = 0x00;

            if ((yHaltCode & 0x01) == (byte)0x01)
            {
                y5475Code |= (byte)0x04;
            }
            if ((yHaltCode & 0x02) == (byte)0x02)
            {
                y5475Code |= (byte)0x20;
            }
            if ((yHaltCode & 0x04) == (byte)0x04)
            {
                y5475Code |= (byte)0x80;
            }
            if ((yHaltCode & 0x08) == (byte)0x08)
            {
                y5475Code |= (byte)0x40;
            }
            if ((yHaltCode & 0x10) == (byte)0x10)
            {
                y5475Code |= (byte)0x10;
            }
            if ((yHaltCode & 0x20) == (byte)0x20)
            {
                y5475Code |= (byte)0x08;
            }
            if ((yHaltCode & 0x40) == (byte)0x40)
            {
                y5475Code |= (byte)0x02;
            }

            return y5475Code;
        }

        protected byte Convert5745CodeToHalt (byte y5475Code)
        {
            byte yHaltCode = 0x00;

            if ((y5475Code & 0x02) == (byte)0x02)
            {
                yHaltCode |= (byte)0x40;
            }
            if ((y5475Code & 0x04) == (byte)0x04)
            {
                yHaltCode |= (byte)0x01;
            }
            if ((y5475Code & 0x08) == (byte)0x08)
            {
                yHaltCode |= (byte)0x20;
            }
            if ((y5475Code & 0x10) == (byte)0x10)
            {
                yHaltCode |= (byte)0x10;
            }
            if ((y5475Code & 0x20) == (byte)0x20)
            {
                yHaltCode |= (byte)0x02;
            }
            if ((y5475Code & 0x40) == (byte)0x40)
            {
                yHaltCode |= (byte)0x08;
            }
            if ((y5475Code & 0x80) == (byte)0x80)
            {
                yHaltCode |= (byte)0x04;
            }

            return yHaltCode;
        }
        #endregion

        #region Validation methods
        private static bool IsLower (char cTest)
        {
            return (cTest >= 'a' && cTest <= 'z');
        }

        private static bool IsAscii (char cTest)
        {
            return cTest >= 0x20 &&
                   cTest <  0x7F;
        }

        private bool AddressWraps (int iAddress, int iLength)
        {
            if (!IsAddressValid (iAddress))
            {
                return false;
            }

            if (iAddress - (iLength - 1) >= 0)
            {
                return false;
            }
            else
            {
                m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_5_PChk_AddressWrap);
                return true;
            }
        }

        private bool IsAddressValid (int iAddress)
        {
            if (iAddress < m_yaMainMemory.Length)
            {
                return true;
            }
            else
            {
                InvalidAddress ();
                return false;
            }
        }

        private bool BinaryAddressesValid (CTwoOperandAddress objTOA, byte yQByte)
        {
            if (!IsAddressValid (objTOA.OperandOneAddress) ||
                !IsAddressValid (objTOA.OperandTwoAddress))
            {
                return false;
            }

            if (AddressWraps (objTOA.OperandOneAddress, (int)yQByte) ||
                AddressWraps (objTOA.OperandTwoAddress, (int)yQByte))
            {
                AddressWraparound ();
                return false;
            }

            return true;
        }

        private bool ZonedAddressesValid (CTwoOperandAddress objTOA, byte yQByte)
        {
            if (!IsAddressValid (objTOA.OperandOneAddress) ||
                !IsAddressValid (objTOA.OperandTwoAddress))
            {
                return false;
            }

            int iOperandTwoLength = (yQByte & 0x0F) + 1,
                iOperandOneLength = (yQByte >> 4) + iOperandTwoLength;

            if (iOperandOneLength > 31 ||
                iOperandTwoLength > 16)
            {
                InvalidQByte ();
                return false;
            }

            if (AddressWraps (objTOA.OperandOneAddress, iOperandOneLength + iOperandOneLength) ||
                AddressWraps (objTOA.OperandTwoAddress, iOperandTwoLength))
            {
                AddressWraparound ();
                return false;
            }

            return true;
        }
        #endregion

        #region Processor Check State Methods
        private void InvalidOpCode ()
        {
            m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_5_PChk_InvalidOpCode);

            WaveRedFlag ("Invalid Op Code");
        }

        private void InvalidQByte ()
        {
            m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_5_PChk_InvalidQByte);

            WaveRedFlag ("Invalid Q Byte");
        }

        private void AddressWraparound ()
        {
            m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_5_PChk_AddressWrap);

            WaveRedFlag ("Address Wraparound");
        }

        private void InvalidAddress ()
        {
            m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_5_PChk_InvalidAddress);

            WaveRedFlag ("Invalid Address");
        }

        private void NoPL2Support ()
        {
            m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_5_PChk_PL2_Unsupported);

            WaveRedFlag ("Attempt to use PL2");
        }

        private void UnsupportedDevice ()
        {
            m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_5_PChk_UnsupportedDevice);

            WaveRedFlag ("Unsupported device");
        }

        private void FunctionNotImplemented ()
        {
            if (!m_bAsyncRun)
            {
                WriteLogOutputLine ("");
                WriteLogOutputLine ("************************************");
                WriteLogOutputLine ("************************************");
                WriteLogOutputLine ("***                              ***");
                WriteLogOutputLine ("***   Function Not Implemented   ***");
                WriteLogOutputLine ("***                              ***");
                WriteLogOutputLine ("************************************");
                WriteLogOutputLine ("************************************");
                WriteLogOutputLine ("");
            }
        }
        #endregion
    }
}
