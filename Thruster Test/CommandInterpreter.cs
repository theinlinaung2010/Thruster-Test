using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroLibrary;

namespace Thruster_Test
{
    class CommandInterpreter
    {
        Bridge theBridge;
        public event Action CommandFinished;
        MicroTimer timer = new MicroTimer();
        int timerFlag;
        double currentMotorSpeed = 0.0;
        double speedIncrement = 0;
        long duration;

        public CommandInterpreter(Bridge _bridge)
        {
            theBridge = _bridge;
            timer.MicroTimerElapsed += timer_TimerTick;
        }

        public void NextCommand(string cmd, List<int> paras)
        {
            switch (cmd)
            {
                case "lf":
                    {
                        int timeSpan = paras[2];
                        int speedSpan = paras[1] - paras[0];
                        speedIncrement = speedSpan * 1.0 / timeSpan; //speed increment in 1 millis
                        duration = timeSpan * 1000; //Timer count in micros
                        timer = new MicroTimer();
                        timer.MicroTimerElapsed += timer_TimerTick;
                        timer.Interval = 1000;
                        currentMotorSpeed = paras[0];
                        timerFlag = 0;
                        timer.Start();
                    }
                    break;
                case "lr":
                    {
                        int timeSpan = paras[2];
                        int speedSpan = paras[1] - paras[0];
                        speedIncrement = speedSpan * 1.0 / timeSpan; //speed increment in 1 millis
                        duration = timeSpan * 1000; //Timer count in micros
                        timer = new MicroTimer();
                        timer.MicroTimerElapsed += timer_TimerTick;
                        timer.Interval = 1000;
                        currentMotorSpeed = paras[0];
                        timerFlag = 1;
                        timer.Start();
                    }
                    break;
                case "fw":
                    theBridge.SendMotorSpeed(paras[0]);
                    if (paras.Count > 1)
                    {
                        timer = new MicroTimer();
                        timer.MicroTimerElapsed += timer_TimerTick;
                        timer.Interval = paras[1] * 1000;
                        timerFlag = 2;
                        timer.Start();
                    }
                    else
                    {
                        CommandFinished();
                    }
                    break;
                case "rv":
                    theBridge.SendMotorSpeed(-paras[0]);
                    if (paras.Count > 1)
                    {
                        timer = new MicroTimer();
                        timer.MicroTimerElapsed += timer_TimerTick;
                        timer.Interval = paras[1] * 1000;
                        timerFlag = 2;
                        timer.Start();
                    }
                    else
                    {
                        CommandFinished();
                    }
                    break;
                case "st":
                    theBridge.SendMotorSpeed(0);
                    if (paras.Count > 0)
                    {
                        timer = new MicroTimer();
                        timer.MicroTimerElapsed += timer_TimerTick;
                        timer.Interval = paras[0] * 1000;
                        timerFlag = 2;
                        timer.Start();
                    }
                    else
                    {
                        CommandFinished();
                    }
                    break;
                case "wt":                    
                    timer = new MicroTimer();
                    timer.MicroTimerElapsed += timer_TimerTick;
                    timer.Interval = paras[0] * 1000;
                    timerFlag = 2;
                    duration = paras[0] * 1000;
                    timer.Start();
                    break;
                case "tr":
                    theBridge.SendTare();
                    CommandFinished();
                    break;
                default:
                    break;
            }
        }

        public void Stop()
        {
            timer.StopAndWait();
        }

        private void timer_TimerTick(object sender, MicroTimerEventArgs timerEventArgs)
        {
            switch (timerFlag)
            {
                case 0:
                    {
                        currentMotorSpeed += speedIncrement;
                        int nextMotorSpeed = (int)Math.Truncate(currentMotorSpeed + speedIncrement);
                        if (nextMotorSpeed != currentMotorSpeed)
                        {
                            theBridge.SendMotorSpeed(nextMotorSpeed);
                        }
                        if (timerEventArgs.ElapsedMicroseconds > duration)
                        {
                            timer.StopAndWait();
                            CommandFinished();
                        }
                    }
                    break;
                case 1:
                    {
                        int nextMotorSpeed = (int)Math.Truncate(currentMotorSpeed + speedIncrement);
                        if (nextMotorSpeed != (int)Math.Truncate(currentMotorSpeed))
                        {
                            theBridge.SendMotorSpeed(-nextMotorSpeed);
                        }
                        currentMotorSpeed += speedIncrement;
                        if (timerEventArgs.ElapsedMicroseconds >= duration)
                        {
                            timer.StopAndWait();
                            CommandFinished();
                        }
                    }
                    break;
                case 2:
                    timer.StopAndWait();
                    CommandFinished();
                    break;
                default:
                    break;
            }
        }
    }
}
