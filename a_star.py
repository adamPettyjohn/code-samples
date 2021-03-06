# This is a python program that allows an agent to compute
# the shortest path from an start point to a goal point
# that goes around rectangular obstacles. Each rectangular obstacle
# has four corners and the robot is only allowed to move from one corner
# to another. We call each corner a "State". A legal move is one from one
# corner to another without going through a rectangle obstacle.
# However, going along the outside is fine and common.

# This is a class for storing problem states.
# pos is the 2D coordinate position of the robot as a tuple
# g_val is the cost so far for A* search
# h_val is the heuristic function value for A* search
# f_val is the sum of g_val and h_val
# succ is the list of successors
# parent is the parent of the node

class State(object):

    def __init__(self, pos = (0,0), g_val = 0, h_val = 0, f_val = 0, succ = [], parent = None):
        self.pos = pos
        self.g_val = g_val
        self.h_val = h_val
        self.f_val = f_val
        self.succ = succ
        self.parent = parent




# 'astar' is a function that gives us the path and cost from start to goal state
# Input: Input file describing the problem instance.
# Output: Solution path and cost for that path.

# The code for parsing the input file to create different useful lists are already written here.
# See the course page for input file structure detail.
def astar(input_file):

    # Opening the input file
    f = open(input_file)

    # Retrieving start location from the first line of the file
    s = f.readline().split()[0:2]
    start = (int(s[0]),int(s[1]))

    # Retrieving goal location from the second line of the file
    s = f.readline().split()[0:2]
    goal = (int(s[0]),int(s[1]))

    # Retrieving number of obstacles.
    n_obstacles = f.readline()

    # Creating a list to store the obstacle corners.
    # They are stored in clockwise order as described in the course web-page
    obstacles = []
    for line in f:
        ln = line.split()

        # Each corner is a tuple of (x,y) location
        top_left = (int(ln[0]),int(ln[1]))
        top_right = (int(ln[2]),int(ln[3]))
        bottom_right = (int(ln[4]),int(ln[5]))
        bottom_left = (int(ln[6]),int(ln[7]))

        #'obstacles' is a list of tuples where each tuple contains four corner points
        obstacles += [(top_left,top_right,bottom_right,bottom_left)]

    # Closing the file. We don't need it anymore
    f.close()

    # Now we will create nodes and lines from all the obstacles.
    # remember the agent is only allowed to stay at one of these nodes
    # 'nodes' is a list of (x,y) tuples containing the x,y position of all nodes
    # 'lines' is a list of tuples where each tuple contains two nodes
    nodes = []; lines = []
    for obstacle in obstacles:
        nodes += [obstacle[0],obstacle[1],obstacle[2],obstacle[3]]
        lines += [(obstacle[0],obstacle[1])]
        lines += [(obstacle[1],obstacle[2])]
        lines += [(obstacle[2],obstacle[3])]
        lines += [(obstacle[3],obstacle[0])]


    # The main A* code will be here. Best of luck :)
    # Thanks :)
    # ============Code I Wrote===================

    # these are the lists that contain the nodes that have states
    open_nodes = [] # states not yet expanded
    closed_nodes = [] # states expanded

    # an initial state is created from the start node
    start_h = pow(pow(goal[0] - start[0], 2) + pow(goal[1] - start[1],2), 0.5)
    
    start_state = State(start, 0, start_h, start_h + 0, [], None)

    # we add our start node to our open list
    open_nodes.append(start_state)

    # we set the current state were testing to the only state we currently have
    current_loc = start_state

    while current_loc.pos[0] != goal[0] or current_loc.pos[1] != goal[1]: # as long as this is true we are not at the goal
        #first we find the children of the state
        current_loc.succ = find_valid_children(current_loc, nodes, lines, obstacles, goal)
        
        # we expanded the state so its time for it to switch lists
        open_nodes.remove(current_loc)
        closed_nodes.append(current_loc)

        # we add the node's childern to the open list as they now have states
        for child in current_loc.succ:
            open_nodes.append(child)

        # now we look for the next state we should expand
        cur_next = open_nodes[0]
        cur_next_dis = open_nodes[0].f_val
        for pos_next in open_nodes:
            if pos_next.f_val < cur_next_dis:
                cur_next = pos_next
                cur_next_dis = pos_next.f_val

        current_loc = cur_next

    # when our while loop breaks we know that current_loc is the goal state so we build the solution using it
    # this list contains the points in the minmal path
    path_points = []

    cur_path_pos = current_loc

    found_start = False
    
    # we backtrace from the goal state to find the path to get there
    while not found_start: # this checks if we have backtraced to the start state
        path_points.append(cur_path_pos)

        if cur_path_pos.parent == None:
            found_start = True
        else:
            cur_path_pos = cur_path_pos.parent

    # now we reverse our list and print out our results
    path_points.reverse()

    titles = 'Point            Cumulative Cost'
    print(titles)

    # this loops through the path and prints the nodes in it
    for i in range(0, len(path_points)):
        stringOfPair = '(%d, %d)           %.4f' % (path_points[i].pos[0], path_points[i].pos[1], path_points[i].g_val)
        print(stringOfPair)



# To implement A*, you might want to use a helper function which will
# find all the valid states you allowed to move to. Valid state means
# that the agent can move to that state without intersecting any rectangle.

# REMEMBER: You need to check whether a move intersects any rectangle.
# We have provided a function that does that checking for you.
# You need to call that function somewhere inside this function.
# Check def find_valid_move(p0,p1,lines,obstacles) to see the
# i/p and o/p of the function.
def find_valid_children(state,nodes,lines,obstacles,goal):

    # Empty list of children that needs to be returned by the function
    children = []


    # this loop loops through the available nodes and if they are valid to move to it creates states for them
    for node in nodes:
        if find_valid_move(state.pos, node, lines, obstacles): # first we check if the move is valid
            # we create the g and h vals using the strait line distance between the nodes
            cal_g = pow(pow(node[0] - state.pos[0], 2) + pow(node[1] - state.pos[1],2), 0.5) + state.g_val
            cal_h = pow(pow(goal[0] - node[0], 2) + pow(goal[1] - node[1],2), 0.5)
            cal_f = cal_g + cal_h
            # we then create the state from the values we have
            valid_child = State(node, cal_g, cal_h, cal_f, [], state)
            children.append(valid_child)

    return children

# ============End Code I Wrote===================


# This function determines if the move from p0 to p1 is valid or not.

# Input: - 'p0' and 'p1' are two tuples containing starting and ending co-ordinate, respectively.
#        - 'lines' is a list of tuples where each tuple is two points. See the construction of 'lines'
#           in the astar function above.
#        - 'obstacles' is a list of tuples where each tuple contains four corner points. See the
#           construction of 'obstacles' in the astar function above.

# Output: - 'True' if the move is valid (does not intersect any obstacle)
#         - 'False' if the move is invalid (intersects a rectangle)

# Detail:  You are not required to understand the detail of this function to implement this assignment.
#          You can just call it directly whenever needed. But you can always dig deeper.
#          On a high level, this function tests if two lines intersect or not. One line
#          is the movement line and other is edge of all obstacles stored in the 'lines'
#          list. As each obstacle is made of four edges (lines), only checking the line
#          intersection is sufficient.

def find_valid_move(p0,p1,lines,obstacles):

    # Test if (p0,p1) is an edge of any obstacle,
    # which is already valid according to the problem definition.
    for obstacle in obstacles:
        if p0 in obstacle and p1 in obstacle:

            # Finding the indices
            i = obstacle.index(p0)
            j = obstacle.index(p1)

            # Make sure that i is the smaller index. This is very important as
            # the nodes are sorted in clockwise order
            if j < i:
                i,j = j,i

            # Indices are consecutive. So this is an edge
            if 1 == (j - i) or (0 == i and len(obstacle)-1 == j):
                return True
            else:
                return False

    # Now we have to find if the line (p0,p1) intersects any obstacle.
    # Obstacles are made of edges (lines). So just checking
    # if (p0,p1) intersects any line will be sufficient.

    for line in lines:
        # skip if p0 or p1 is an endpoint of this line
        if p0 in line or p1 in line:
            continue

        # We utilized vector math to figure out if any two lines intersects each other
        # The math details can be found here:
        # see http://www.cs.iastate.edu/~cs518/handouts/segment-intersect.ppt (Slide 4)
        r0 = sub(line[0],line[1])
        c0 = cross(sub(p0,line[1]),r0)
        c1 = cross(sub(p1,line[1]),r0)

        r1 = sub(p0,p1)
        c2 = cross(sub(line[0],p1),r1)
        c3 = cross(sub(line[1],p1),r1)

        # If the sign does not match, its an intersection
        if (c0 == 0 or c1 == 0 or sign(c0) != sign(c1)) and (c2 == 0 or c3 == 0 or sign(c2) != sign(c3)):
            return False

    return True


# The following three functions (sign, sub, and cross) are small helper functions used to
# implement find_valid_move function.

# returns the sign of a number as -1,0,1
def sign(x):
    if x == 0:
        return 0
    elif x < 0:
        return -1
    elif x >0:
        return 1
    else:
        return None

# Subtracting one point from another
def sub(a,b):
    return (a[0]-b[0],a[1]-b[1])

# return the vector cross product
# only need 2D case, so returns z component as scalar
# inputs are in form ((x0,y0),(x1,y1))
def cross(a,b):
    return a[0]*b[1]-a[1]*b[0]


# This is the main function. It calls the astar function and prints the result
# in appropriate format as described in the course web-page.
if __name__ == "__main__":

    # 'result' will store the output of the 'astar' function.
    # You need to print the result in appropriate format
    # as described in the course web-page
    result = astar('data1.txt')


