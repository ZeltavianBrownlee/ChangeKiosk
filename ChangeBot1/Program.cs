using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeBot1
{
    class Program
    {
        #region GLOBAL VARIABLES
        struct Bank 
        {
            public int stored; // how many of each is stored in the bank (upon start up)
        }
        static Bank[] money;
        #endregion

        static void Main(string[] args)
        {
            #region VARIABLES
            //Arrays cash
            string[] moneyNames = { "One Hundred", "Fifty", "Twenty", "Ten", "Five", "Dollar", "Half Dollar", "Quarter", "Dime", "Nickel", "Penny" };
            decimal[] moneyValue = { 100.00m, 50.00m, 20.00m, 10.00m, 5.00m, 1.00m, .50m, .25m, .10m, .05m, .01m };
            int[] drawerBillCount = { 1, 2, 4, 5, 8, 21, 5, 20, 28, 20, 20 };
            int[] drawerBillCount1 = { 1, 1, 0, 1, 4, 1, 1, 1, 1, 1, 1 }; //test money amount array
            //int[] drawerBillCount1 = { 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1 };
            money = new Bank[moneyNames.Length];//ccreate an instance of money

            //declare and intialize variables 
            decimal itemPrice, purchasePrice = 0.00m;
            decimal remaining = 0.00m;
            decimal payment, totalPayment = 0.00m;
            int itemNum = 1;
            int paymentNum = 1;
            string input, formattedInput = "";
            decimal changeDue = 0.00m;
            int index = 0;
            bool transactionOn = true;
            string transactionResponse = "";
            #endregion


            #region  BOOT UP DATA FOR CASH CHECK
            //SETUP BOOT UP CASH
            while (index < 11)
            //store preloaded information into kiosk
            {
                money[index].stored = drawerBillCount1[index];//global money drawer array set to local money drawer
                index++;
            }
            #endregion
            while (transactionOn)//loop to allow customers to continue making transactions
            {

                Console.WriteLine("You may begin scanning your items!\n");

                // loop to get price of items being purchased
                do
                {
                    Console.Write("Item {0}: ", itemNum);


                    //break if no input value
                    input = Console.ReadLine();
                    if (input == "")
                    {
                        break;
                    }

                    //format the discount price to have two decimal places
                    formattedInput = string.Format("{0:0.00}", input);

                    //convert input price to double for calculations
                    itemPrice = Convert.ToDecimal(input);

                    Console.WriteLine("Please place item in bagging area!\n");
                    itemNum += 1;
                    purchasePrice += itemPrice;
                } while (input != "");//end do while loop

                // Total price display
                Console.Write("\n\nTotal {0:c}", purchasePrice);

                //loop to accept payment amounts 
                while (totalPayment <= purchasePrice)
                {
                    Console.Write("\n\nPayment {0}  ", paymentNum);
                    payment = Convert.ToDecimal(Console.ReadLine());


                    //check payment for valid payment amount
                    bool paymentCheck = PaymentCheck(payment, moneyValue, drawerBillCount1); //set bool equal to PaymentCheck function

                    
                    if(paymentCheck == false)
                    {
                        Console.WriteLine("Please enter a valid payment amount.");
                        totalPayment = totalPayment - payment;
                        paymentNum -= 1;

                    }

                    totalPayment += payment;
                    remaining = purchasePrice - totalPayment; //subtract payment from purchase price and set it to remaining
                    paymentNum += 1;

                    if (totalPayment > purchasePrice)
                    {
                        break;//exit loop if total payment is higher than sum of items
                    }//end if 

                    //Remaining change to be payed
                    Console.Write("Remaining  {0:c}", remaining);
                }//end while loop

                remaining = Math.Round(remaining, 2);
                //Absoulte value of change (gets rid of negative sign)
                changeDue = Math.Abs(remaining);

                //Change due
                Console.Write("\n\nChange     {0:c}", changeDue);
                Console.Write("\n_ _ _ _ \n\n");

                //call enoughChange function
                bool enoughChange = EnoughChange(changeDue, moneyValue);

                if (enoughChange == false)//there is enough money
                {
                    //display message if enoughChange equals false (there is not enough money)
                    Console.WriteLine("Kiosk does not have enough money to dispense change! Please enter another form of payment.");
                }//end if

                ChangeDispense(changeDue, enoughChange, moneyValue, drawerBillCount1);

                paymentNum = 1;//reset paymentNum
                itemNum = 1;//reset itemNum

                Console.WriteLine("Do you want to do another transaction? Please enter Y or N");
                transactionResponse = Console.ReadLine();

                if (transactionResponse == "No" || transactionResponse == "N" || transactionResponse == "n" || transactionResponse == "no")
                {
                    transactionOn = false;//turns off transaction loop
                }//end if
                Console.WriteLine("\n\n");

            }//end while loop
            Console.ReadKey();
        }//end main function


        #region DISPENSE CHANGE
        public static void ChangeDispense(decimal change_needed, bool enoughChange, decimal[]moneyAmount, int[]drawerAmount)
        {
            int count = 0;
            //loop through to display dispensed money
            while (change_needed > 0 && enoughChange == true)
            { 
                if (change_needed >= moneyAmount[count] && drawerAmount[count] >0)
                {
                    change_needed -= moneyAmount[count];
                    change_needed = Math.Round(change_needed, 2);
                    drawerAmount[count] -= 1;
                        
                   Console.WriteLine($"{moneyAmount[count]:c} dispensed");//display the dispensed change


                    if (change_needed == 0)
                    {
                        Console.WriteLine("\n-----Transaction Complete-----");

                    }
                }
                else if (drawerAmount[count] <0)
                {
                    count += 2;
                }

                else
                {
                    count += 1;

                }//end if     
               
            }//end while loop
        }//end function
        #endregion

        #region CHECK CHANGE
        public static bool EnoughChange(decimal change_due, decimal[] moneyValue)
        {
            int i = 0;
            //loop through to kiosk for proper change
            while (change_due > 0  )
            {
                if(money[i].stored== 0)
                {
                    i +=1;
                }
                else if (change_due >= moneyValue[i] && money[i].stored >0)
                {
                    change_due -= (moneyValue[i]);
                    change_due= Math.Round(change_due, 2);
                    //subtracts array element to check for same bill/coin in case there is more than 1
                    money[i].stored -= 1;
                    //increments to next array element if stored is equal to zero
                    if (money[i].stored == 0 && i != 10 )
                    {
                        i += 1;

                        if(i > 10 )
                        {
                            return false;
                        }//end nested if    


                    }//end nested if
                    //returns false if money makes it to the end of money drawer without zeroing out
                    if(money[10].stored == 0 && i == 10)
                    {
                        return false;
                    }//end nested if
                    //returns true if change zeros out
                    if( money[i].stored >= 0 && change_due == 0)
                    {
                        return true;// make bool true if no change remains and bill/coin count is zero or more

                    }//end nested if
                }
                else
                {
                    i += 1;
                    if(i>10)
                    {
                        return false;
                    }//end nested if
                }//end if

            }//end while loop
            return false;
        }//end function  

        #endregion

        #region PAYMENT CHECK
        public static bool PaymentCheck (decimal payments, decimal[] moneyValue, int[] moneyStored)
        {
            for(int x = 0; x<moneyValue.Length;x++)
            {
                if(payments == moneyValue[x])
                {
                    moneyValue[x] += 1;//increase value of money drawer
                    return true;
                }
                if(payments != moneyValue[x] && x == 10)
                {
                     return false;
                }
               
            }


            return false;
        }

        #endregion
    }//end program class
}//end namespace

